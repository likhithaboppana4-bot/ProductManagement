namespace LibraryManagement.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Genre { get; set; }
        public int Quantity { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string ShelfLocation { get; set; }

    }
}
