using Carbase.Models.Car;
using Microsoft.EntityFrameworkCore;

namespace Carbase.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("citext");

            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(c => c.Brand)
                    .HasColumnType("citext");

                entity.Property(c => c.Model)
                    .HasColumnType("citext");

                entity.Property(c => c.Tuner)
                    .HasColumnType("citext");

                entity.HasIndex(c => new
                {
                    c.Brand,
                    c.Model,
                    c.Year,
                    c.Tuner
                })
                .IsUnique()
                .AreNullsDistinct(false);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
