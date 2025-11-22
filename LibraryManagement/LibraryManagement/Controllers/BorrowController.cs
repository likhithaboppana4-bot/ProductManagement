using LibraryManagement.DTOs;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/borrow")]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _borrowService;
        private readonly ILogger<BorrowController> _logger;

        public BorrowController(IBorrowService borrowService, ILogger<BorrowController> logger)
        {
            _borrowService = borrowService;
            _logger = logger;
        }

        // POST: api/borrow/borrow
        [HttpPost("borrow")]
        public async Task<IActionResult> Borrow([FromBody] BorrowRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var record = await _borrowService.BorrowBookAsync(dto);
                return Ok(record);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { message = knf.Message });
            }
            catch (InvalidOperationException inv)
            {
                return BadRequest(new { message = inv.Message });
            }
        }

        // POST: api/borrow/return
        [HttpPost("return")]
        public async Task<IActionResult> Return([FromBody] ReturnRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var (success, fine, isOverdue) = await _borrowService.ReturnBookAsync(dto);
                if (!success) return BadRequest(new { message = "No active borrow record found for this borrower/book" });

                return Ok(new { fine, isOverdue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning book");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/borrow/active/{borrowerId}
        [HttpGet("active/{borrowerId:int}")]
        public async Task<IActionResult> GetActive(int borrowerId)
        {
            var recs = await _borrowService.GetActiveBorrowRecords(borrowerId);
            return Ok(recs);
        }

        // GET: api/borrow/history/{borrowerId}
        [HttpGet("history/{borrowerId:int}")]
        public async Task<IActionResult> GetHistory(int borrowerId)
        {
            // If you have a method for history; if not, reuse active or query BorrowRecords from db in service
            var recs = await _borrowService.GetActiveBorrowRecords(borrowerId);
            return Ok(recs);
        }
    }
}
