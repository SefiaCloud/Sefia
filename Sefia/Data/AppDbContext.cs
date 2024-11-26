using Microsoft.EntityFrameworkCore;
using Sefia.Entities;

namespace Sefia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LoginHistory> LoginHistorys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginHistory>(entity =>
            {
                entity.HasOne(lh => lh.User)
                      .WithMany(u => u.History)
                      .HasForeignKey(lh => lh.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
