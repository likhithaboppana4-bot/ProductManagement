using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Author { get; set; }

        [MaxLength(20)]
        public string ISBN { get; set; } 

        [MaxLength(100)]
        public string Genre { get; set; }

        public int Quantity { get; set; }

        public DateTime? PublishedDate { get; set; }

        [MaxLength(200)]
        public string Publisher { get; set; }

        [MaxLength(50)]
        public string Language { get; set; }

        [MaxLength(50)]
        public string ShelfLocation { get; set; }
    }
}
