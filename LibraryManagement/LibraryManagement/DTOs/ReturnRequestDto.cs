using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs
{
    public class ReturnRequestDto
    {
        [Required] public int BorrowerId { get; set; }
        [Required] public int BookId { get; set; }
    }
}
