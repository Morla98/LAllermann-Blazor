using Microsoft.EntityFrameworkCore;

namespace LAllermannREST.Models
{
    public class PasswordContext : DbContext
    {
        public DbSet<Password> Password { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=lallermann_rest.db");
    }
}
