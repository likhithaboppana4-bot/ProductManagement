using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IExternalApiService
    {
        Task<string> GetBookInfoRawAsync(string isbn);
        Task<IEnumerable<ExternalApiLog>> GetLogsAsync(int page, int pageSize);
    }
}
