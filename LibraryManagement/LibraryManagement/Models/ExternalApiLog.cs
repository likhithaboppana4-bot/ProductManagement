namespace LibraryManagement.Models
{
    public class ExternalApiLog
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public DateTime CalledAt { get; set; }
        public int DurationMs { get; set; }
        public bool IsSuccess { get; set; }
    }
}
