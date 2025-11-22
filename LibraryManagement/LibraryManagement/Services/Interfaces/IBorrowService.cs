using LibraryManagement.DTOs;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBorrowService
    {
        Task<BorrowRecord> BorrowBookAsync(BorrowRequestDto dto);
        Task<(bool Success, decimal Fine, bool IsOverdue)> ReturnBookAsync(ReturnRequestDto dto);
        Task<IEnumerable<BorrowRecord>> GetActiveBorrowRecords(int borrowerId);
    }
}
