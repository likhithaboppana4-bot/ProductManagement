using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class ProductService:IProductService
    {
        private readonly ApplicationDbContext _db;
        public ProductService(ApplicationDbContext db) => _db = db;

        public async Task<Product> CreateAsync(ProductCreateDto dto)
        {
            // check SKU unique
            if (await _db.Products.AnyAsync(p => p.SKU == dto.SKU))
                throw new InvalidOperationException("SKU already exists.");

            var p = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                SKU = dto.SKU,
                Category = dto.Category,
                Price = dto.Price,
                QuantityInStock = dto.QuantityInStock,
                Manufacturer = dto.Manufacturer,
                Weight = dto.Weight,
                Dimensions = dto.Dimensions,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _db.Products.Add(p);
            await _db.SaveChangesAsync();
            return p;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return false;
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Product?> GetByIdAsync(int id) => await _db.Products.FindAsync(id);

        public async Task<PagedResult<Product>> SearchAsync(string search, string category, int page, int pageSize, string sort)
        {
            var q = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.Name.Contains(search) || x.Description.Contains(search) || x.SKU.Contains(search));
            if (!string.IsNullOrWhiteSpace(category))
                q = q.Where(x => x.Category == category);

            // sort parsing: e.g., "price_desc", "createdAt_asc"
            q = sort?.ToLower() switch
            {
                "price_asc" => q.OrderBy(p => p.Price),
                "price_desc" => q.OrderByDescending(p => p.Price),
                "createdat_asc" => q.OrderBy(p => p.CreatedAt),
                "createdat_desc" => q.OrderByDescending(p => p.CreatedAt),
                _ => q.OrderByDescending(p => p.CreatedAt)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Product> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public async Task<Product?> UpdateAsync(int id, ProductCreateDto dto)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return null;

            // If SKU changed, ensure uniqueness
            if (!string.Equals(p.SKU, dto.SKU, StringComparison.OrdinalIgnoreCase))
            {
                if (await _db.Products.AnyAsync(x => x.SKU == dto.SKU && x.ProductId != id))
                    throw new InvalidOperationException("SKU conflict.");
                p.SKU = dto.SKU;
            }

            p.Name = dto.Name;
            p.Description = dto.Description;
            p.Category = dto.Category;
            p.Price = dto.Price;
            p.QuantityInStock = dto.QuantityInStock;
            p.Manufacturer = dto.Manufacturer;
            p.Weight = dto.Weight;
            p.Dimensions = dto.Dimensions;

            await _db.SaveChangesAsync();
            return p;
        }
    }
}
