using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class BookService:IBookService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<BookService> _logger;

        public BookService(ApplicationDbContext db, ILogger<BookService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Book> GetAsync(int id) =>
            await _db.Books.FindAsync(id);

        public async Task<(IEnumerable<Book> Items, int Total)> SearchAsync(string q, string author, string genre, int page, int pageSize)
        {
            var query = _db.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(b => b.Title.Contains(q));
            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(b => b.Author.Contains(author));
            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(b => b.Genre.Contains(genre));

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Book> CreateAsync(BookCreateDto dto)
        {
            var b = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                ISBN = dto.ISBN,
                Genre = dto.Genre,
                Quantity = dto.Quantity,
                PublishedDate = dto.PublishedDate,
                Publisher = dto.Publisher,
                Language = dto.Language,
                ShelfLocation = dto.ShelfLocation
            };
            _db.Books.Add(b);
            await _db.SaveChangesAsync();
            return b;
        }

        public async Task<Book> UpdateAsync(int id, BookCreateDto dto)
        {
            var b = await _db.Books.FindAsync(id);
            if (b == null) return null;
            b.Title = dto.Title; b.Author = dto.Author; b.ISBN = dto.ISBN; b.Genre = dto.Genre;
            b.Quantity = dto.Quantity; b.PublishedDate = dto.PublishedDate; b.Publisher = dto.Publisher;
            b.Language = dto.Language; b.ShelfLocation = dto.ShelfLocation;
            await _db.SaveChangesAsync();
            return b;
        }

        public async Task DeleteAsync(int id)
        {
            var b = await _db.Books.FindAsync(id);
            if (b == null) return;
            _db.Books.Remove(b);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsAvailableAsync(int bookId)
        {
            var b = await _db.Books.FindAsync(bookId);
            return b != null && b.Quantity > 0;
        }
    }
}
