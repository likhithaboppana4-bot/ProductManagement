using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.Services
{
    public class ExternalApiService:IExternalApiService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ExternalApiService> _logger;
        private readonly IConfiguration _config;
        private readonly TimeSpan _ttl;

        public ExternalApiService(ApplicationDbContext db, IMemoryCache cache, IHttpClientFactory clientFactory,
            ILogger<ExternalApiService> logger, IConfiguration config)
        {
            _db = db;
            _cache = cache;
            _clientFactory = clientFactory;
            _logger = logger;
            _config = config;
            var minutes = _config.GetValue<int?>("ExternalApi:CacheMinutes") ?? 60;
            _ttl = TimeSpan.FromMinutes(minutes);
        }

        public async Task<string> GetBookInfoRawAsync(string isbn)
        {
            var key = $"bookinfo_{isbn}";
            if (_cache.TryGetValue(key, out string cached)) return cached;

            var http = _clientFactory.CreateClient();
            // Example: try using Open Library or mock if configured
            var useMock = _config.GetValue<bool?>("ExternalApi:UseMock") ?? true;
            var sw = Stopwatch.StartNew();
            var log = new ExternalApiLog { Endpoint = $"GET /external/bookinfo/{isbn}", CalledAt = DateTime.UtcNow };

            try
            {
                string responseContent;
                if (useMock)
                {
                    // Mocked result
                    responseContent = $"{{ \"isbn\":\"{isbn}\", \"title\":\"Mocked Title for {isbn}\", \"source\":\"mock\" }}";
                }
                else
                {
                    var res = await http.GetAsync($"https://openlibrary.org/isbn/{isbn}.json");
                    responseContent = await res.Content.ReadAsStringAsync();
                }

                sw.Stop();
                log.DurationMs = (int)sw.ElapsedMilliseconds;
                log.ResponseBody = responseContent;
                log.IsSuccess = true;
                _db.ExternalApiLogs.Add(log);

                // Optionally save parsed data to ExternalBookInfo table (not shown)
                await _db.SaveChangesAsync();

                _cache.Set(key, responseContent, _ttl);
                return responseContent;
            }
            catch (Exception ex)
            {
                sw.Stop();
                log.DurationMs = (int)sw.ElapsedMilliseconds;
                log.ResponseBody = ex.ToString();
                log.IsSuccess = false;
                _db.ExternalApiLogs.Add(log);
                await _db.SaveChangesAsync();
                _logger.LogError(ex, "External API call failed");
                throw;
            }
        }

        public async Task<IEnumerable<ExternalApiLog>> GetLogsAsync(int page, int pageSize)
        {
            return await _db.ExternalApiLogs
                .OrderByDescending(x => x.CalledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
