using LAllermannShared.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAllermannREST.Data
{
    public class UserContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Role> Roles { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=lallermann_rest.db");
    }
}
