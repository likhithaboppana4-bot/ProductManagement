using LibraryManagement.DTOs;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _productService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "SKU conflict");
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _productService.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _productService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // GET: api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _productService.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        // GET: api/products?search=&category=&page=&pageSize=&sort=
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "createdAt_desc")
        {
            var result = await _productService.SearchAsync(search, category, page, pageSize, sort);
            return Ok(new { items = result.Items, total = result.TotalCount, page, pageSize });
        }
    }
}
