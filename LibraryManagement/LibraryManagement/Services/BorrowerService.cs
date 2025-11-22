using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class BorrowerService:IBorrowerService
    {
        private readonly ApplicationDbContext _db;
        public BorrowerService(ApplicationDbContext db) => _db = db;

        public async Task<Borrower> CreateAsync(Borrower dto)
        {
            // ensure unique MembershipId
            if (await _db.Borrowers.AnyAsync(b => b.MembershipId == dto.MembershipId))
                throw new InvalidOperationException("MembershipId already exists.");

            _db.Borrowers.Add(dto);
            await _db.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _db.Borrowers.FindAsync(id);
            if (e == null) return false;
            _db.Borrowers.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Borrower>> GetAllAsync() => await _db.Borrowers.ToListAsync();

        public async Task<Borrower?> GetByIdAsync(int id) => await _db.Borrowers.FindAsync(id);

        public async Task<Borrower?> UpdateAsync(int id, Borrower dto)
        {
            var b = await _db.Borrowers.FindAsync(id);
            if (b == null) return null;
            b.Name = dto.Name;
            b.ContactNumber = dto.ContactNumber;
            b.Email = dto.Email;
            b.Address = dto.Address;
            b.MembershipStart = dto.MembershipStart;
            b.MembershipExpiry = dto.MembershipExpiry;
            // do not change membership id arbitrarily, or add checks
            await _db.SaveChangesAsync();
            return b;
        }
    }
}
