using Microsoft.EntityFrameworkCore;

namespace PerformanceSqLiteVsJsonFile
{
    public class SqLiteDataContext : DbContext
    {
        public DbSet<StockEntity> Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DbStocks.db"); // SQLite database file name
        }
    }
}
