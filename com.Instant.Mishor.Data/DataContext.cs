using com.Instant.Mishor.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace com.Instant.Mishor.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Destination> Destinations { get; set; }

        public virtual DbSet<Hotel> Hotels { get; set; }

        public virtual DbSet<HotelDestination> HotelDestinations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Database=Medici;User Id=sa;Password=12345;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Mishor");

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.HasMany(e => e.Destinations)
                    .WithOne(e => e.Country);

                entity.ToTable("Countries", "Mishor");
            });

            modelBuilder.Entity<Destination>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.HasOne(e => e.Country)
                    .WithMany(e => e.Destinations);

                entity.HasMany(e => e.Hotels)
                .WithMany(e => e.Destinations)
                .UsingEntity<HotelDestination>();

                entity.ToTable("Destinations", "Mishor");
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.HasMany(e => e.Destinations)
                    .WithMany(e => e.Hotels)
                    .UsingEntity<HotelDestination>();

                entity.ToTable("Hotels", "Mishor");
            });

            modelBuilder.Entity<HotelDestination>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                //entity.HasKey(e => new { e.HotelId, e.DestinationId })
                //    .IsClustered(false);

                entity.HasOne(e => e.Hotel)
                    .WithMany(e => e.HotelDestinations);

                entity.HasOne(e => e.Destination)
                    .WithMany(e => e.HotelDestinations);
            });
        }
    }
}