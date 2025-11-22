using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs
{
    public class BorrowRequestDto
    {
        [Required] public int BorrowerId { get; set; }
        [Required] public int BookId { get; set; }
        [Range(1, 365)] public int Days { get; set; }
    }
}
