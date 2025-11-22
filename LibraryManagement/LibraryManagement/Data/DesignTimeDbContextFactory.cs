using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class DesignTimeDbContextFactory
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // SQL Server connection string
            var connectionString = "Server=localhost;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
