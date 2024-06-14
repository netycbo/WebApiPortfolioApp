using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.Data.Entinities;

namespace WebApiPortfolioApp.Data
{
    public class TemporaryDbContext: DbContext
    {
        public TemporaryDbContext(DbContextOptions<TemporaryDbContext> options) : base(options)
        {
        }
        public DbSet<TemporaryProduct> TemporaryProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=temporary.db");
        }
    }
}
