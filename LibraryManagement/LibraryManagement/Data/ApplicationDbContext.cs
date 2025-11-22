using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ExternalApiLog> ExternalApiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Borrower>()
                .HasIndex(b => b.MembershipId)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique(false);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.Book).WithMany()
                .HasForeignKey(br => br.BookId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.Borrower).WithMany()
                .HasForeignKey(br => br.BorrowerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
