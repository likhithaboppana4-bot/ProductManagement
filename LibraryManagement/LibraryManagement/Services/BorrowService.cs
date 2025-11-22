using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class BorrowService:IBorrowService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<BorrowService> _logger;
        private const decimal FinePerDay = 10m; // currency units per overdue day

        public BorrowService(ApplicationDbContext db, ILogger<BorrowService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<BorrowRecord> BorrowBookAsync(BorrowRequestDto dto)
        {
            // validations
            var borrower = await _db.Borrowers.FindAsync(dto.BorrowerId)
                ?? throw new KeyNotFoundException("Borrower not found.");

            if (borrower.MembershipExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("Membership expired.");

            var book = await _db.Books.FindAsync(dto.BookId)
                ?? throw new KeyNotFoundException("Book not found.");

            if (book.Quantity <= 0)
                throw new InvalidOperationException("Book not available.");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                book.Quantity -= 1;
                var now = DateTime.UtcNow;
                var record = new BorrowRecord
                {
                    BorrowerId = dto.BorrowerId,
                    BookId = dto.BookId,
                    BorrowDate = now,
                    DueDate = now.AddDays(dto.Days),
                    IsOverdue = false,
                    FineAmount = 0
                };
                _db.BorrowRecords.Add(record);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error borrowing book");
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<(bool Success, decimal Fine, bool IsOverdue)> ReturnBookAsync(ReturnRequestDto dto)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var record = await _db.BorrowRecords
                    .Where(r => r.BorrowerId == dto.BorrowerId && r.BookId == dto.BookId && r.ReturnDate == null)
                    .OrderByDescending(r => r.BorrowDate)
                    .FirstOrDefaultAsync();

                if (record == null) return (false, 0m, false);

                record.ReturnDate = DateTime.UtcNow;
                if (record.ReturnDate > record.DueDate)
                {
                    var daysLate = (int)Math.Ceiling((record.ReturnDate.Value - record.DueDate).TotalDays);
                    record.IsOverdue = true;
                    record.FineAmount = daysLate * FinePerDay;
                }
                else
                {
                    record.IsOverdue = false;
                    record.FineAmount = 0;
                }

                var book = await _db.Books.FindAsync(dto.BookId);
                if (book != null) book.Quantity += 1;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return (true, record.FineAmount, record.IsOverdue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning book");
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<BorrowRecord>> GetActiveBorrowRecords(int borrowerId)
        {
            return await _db.BorrowRecords
                .Include(r => r.Book)
                .Where(r => r.BorrowerId == borrowerId && r.ReturnDate == null)
                .ToListAsync();
        }
    }
}
