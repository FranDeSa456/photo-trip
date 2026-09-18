using Microsoft.EntityFrameworkCore;
using PhotoTrip.DAL.Entities;

namespace PhotoTrip.DAL.Data
{
    public class PhotoDbContext(DbContextOptions<PhotoDbContext> options) : DbContext(options)
    {
        public DbSet<Region> Regions { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
                .ToTable(t => t.HasCheckConstraint("CK_Review_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5"));
        }
    }
}
