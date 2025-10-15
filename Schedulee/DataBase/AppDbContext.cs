using Microsoft.EntityFrameworkCore;

namespace Schedulee.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ScheduleeDB.db");
        }
    }

}