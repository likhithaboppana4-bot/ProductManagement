using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/external")]
    public class ExternalApiController : ControllerBase
    {
        private readonly IExternalApiService _external;
        private readonly ILogger<ExternalApiController> _logger;

        public ExternalApiController(IExternalApiService external, ILogger<ExternalApiController> logger)
        {
            _external = external;
            _logger = logger;
        }

        // GET: api/external/bookinfo/{isbn}
        [HttpGet("bookinfo/{isbn}")]
        public async Task<IActionResult> GetBookInfo(string isbn)
        {
            try
            {
                var rawJson = await _external.GetBookInfoRawAsync(isbn);
                return Ok(new { isbn, data = rawJson });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External API error");
                return StatusCode(500, new { message = "Failed to fetch external book info" });
            }
        }

        // GET: api/external/logs?page=1&pageSize=20
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var logs = await _external.GetLogsAsync(page, pageSize);
            return Ok(new { items = logs, page, pageSize });
        }
    }
}
