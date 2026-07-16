using Microsoft.EntityFrameworkCore;
using TravelBook.Models;

namespace TravelBook.DataTraditions
{
    public class TravelBookDbContext : DbContext
    {
        public DbSet<Tradition> Traditions { get; set; } = null!;

        public TravelBookDbContext()
        {
            // EnsureCreated автоматично створює актуальну базу без перевірки міграцій,
            // що назавжди прибере помилку PendingModelChangesWarning
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=travelbook.db");
        }
    }
}