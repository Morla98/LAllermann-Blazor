using LAllermannShared.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAllermannREST.Data
{
    public class PasswordContext : DbContext
    {
        public DbSet<Password> Password { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Password>()
				.HasOne<User>()
				.WithMany()
				.HasForeignKey(p => p.UserId)
				.IsRequired();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=lallermann_rest.db");
    }
}
