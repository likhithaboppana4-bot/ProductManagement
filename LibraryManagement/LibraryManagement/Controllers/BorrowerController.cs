using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/borrowers")]
    public class BorrowerController : ControllerBase
    {
        private readonly IBorrowerService _borrowerService;
        private readonly ILogger<BorrowerController> _logger;

        public BorrowerController(IBorrowerService borrowerService, ILogger<BorrowerController> logger)
        {
            _borrowerService = borrowerService;
            _logger = logger;
        }

        // POST: api/borrowers
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Borrower dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _borrowerService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Create borrower conflict");
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/borrowers/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var b = await _borrowerService.GetByIdAsync(id);
            if (b == null) return NotFound();
            return Ok(b);
        }

        // GET: api/borrowers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _borrowerService.GetAllAsync();
            return Ok(list);
        }

        // PUT: api/borrowers/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Borrower dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _borrowerService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/borrowers/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _borrowerService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
