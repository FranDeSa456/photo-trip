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

            modelBuilder.Entity<Region>().HasData(
                new Region { Id = 1, Name = "Abruzzo" },
                new Region { Id = 2, Name = "Basilicata" },
                new Region { Id = 3, Name = "Calabria" },
                new Region { Id = 4, Name = "Campania" },
                new Region { Id = 5, Name = "Emilia-Romagna" },
                new Region { Id = 6, Name = "Friuli-Venezia Giulia" },
                new Region { Id = 7, Name = "Lazio" },
                new Region { Id = 8, Name = "Liguria" },
                new Region { Id = 9, Name = "Lombardia" },
                new Region { Id = 10, Name = "Marche" },
                new Region { Id = 11, Name = "Molise" },
                new Region { Id = 12, Name = "Piemonte" },
                new Region { Id = 13, Name = "Puglia" },
                new Region { Id = 14, Name = "Sardegna" },
                new Region { Id = 15, Name = "Sicilia" },
                new Region { Id = 16, Name = "Toscana" },
                new Region { Id = 17, Name = "Trentino-Alto Adige" },
                new Region { Id = 18, Name = "Umbria" },
                new Region { Id = 19, Name = "Valle d'Aosta" },
                new Region { Id = 20, Name = "Veneto" });
        }
    }
}
