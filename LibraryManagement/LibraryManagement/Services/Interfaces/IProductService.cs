using LibraryManagement.DTOs;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(int id);
        Task<PagedResult<Product>> SearchAsync(string search, string category, int page, int pageSize, string sort);
        Task<Product> CreateAsync(ProductCreateDto dto);
        Task<Product?> UpdateAsync(int id, ProductCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
