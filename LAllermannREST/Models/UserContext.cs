using Microsoft.EntityFrameworkCore;

namespace LAllermannREST.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> User { get; set; }
     

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=lallermann_rest.db");
    }
}
