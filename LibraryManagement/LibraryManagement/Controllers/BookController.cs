using LibraryManagement.DTOs;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // ----------------------------------------------------
        // GET: /api/books/{id}
        // ----------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var book = await _bookService.GetAsync(id);
            if (book == null)
                return NotFound(new { message = "Book not found" });

            return Ok(book);
        }

        // ----------------------------------------------------
        // GET: /api/books/search?q=&author=&genre=&page=&pageSize=
        // ----------------------------------------------------
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            string? q = null,
            string? author = null,
            string? genre = null,
            int page = 1,
            int pageSize = 10)
        {
            var result = await _bookService.SearchAsync(q, author, genre, page, pageSize);

            return Ok(new
            {
                items = result.Items,
                total = result.Total,
                page,
                pageSize
            });
        }

        // ----------------------------------------------------
        // POST: /api/books  (Create)
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // ----------------------------------------------------
        // PUT: /api/books/{id}  (Update)
        // ----------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _bookService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = "Book not found" });

            return Ok(updated);
        }

        // ----------------------------------------------------
        // DELETE: /api/books/{id}
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookService.DeleteAsync(id);
            return Ok(new { message = "Book deleted successfully" });
        }

        // ----------------------------------------------------
        // GET: /api/books/{id}/availability
        // ----------------------------------------------------
        [HttpGet("{id}/availability")]
        public async Task<IActionResult> CheckAvailability(int id)
        {
            bool available = await _bookService.IsAvailableAsync(id);
            return Ok(new { bookId = id, isAvailable = available });
        }
    }
}
