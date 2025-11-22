using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Borrower
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string ContactNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string MembershipId { get; set; } 

        [MaxLength(500)]
        public string Address { get; set; }

        public DateTime MembershipStart { get; set; }
        public DateTime MembershipExpiry { get; set; }
    }
}
