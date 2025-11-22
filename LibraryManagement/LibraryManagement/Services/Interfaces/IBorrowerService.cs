using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBorrowerService
    {
        Task<Borrower?> GetByIdAsync(int id);
        Task<Borrower> CreateAsync(Borrower dto);
        Task<Borrower?> UpdateAsync(int id, Borrower dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Borrower>> GetAllAsync();
    }
}
