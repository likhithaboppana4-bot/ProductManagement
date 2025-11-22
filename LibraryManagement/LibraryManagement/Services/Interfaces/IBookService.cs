using LibraryManagement.DTOs;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBookService
    {
        Task<Book> GetAsync(int id);
        Task<(IEnumerable<Book> Items, int Total)> SearchAsync(string query, string author, string genre, int page, int pageSize);
        Task<Book> CreateAsync(BookCreateDto dto);
        Task<Book> UpdateAsync(int id, BookCreateDto dto);
        Task DeleteAsync(int id);
        Task<bool> IsAvailableAsync(int bookId);
    }
}
