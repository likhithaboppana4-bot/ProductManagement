namespace LibraryManagement.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        public int BorrowerId { get; set; }
        public Borrower Borrower { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
        public decimal FineAmount { get; set; }
    }
}
