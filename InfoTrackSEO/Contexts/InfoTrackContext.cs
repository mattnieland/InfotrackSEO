using InfoTrackSEO.Models;
using InfoTrackSEO.Providers;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackSEO.Contexts;

public class InfoTrackContext : DbContext
{
    private readonly string config = SecretProviders.GetConfig();
    public virtual DbSet<TrackedUrls>? TrackedUrls { get; set; }
    public virtual DbSet<TrackedSearchTerms>? TrackedSearch { get; set; }
    public virtual DbSet<TrackedSearchData>? TrackedSearchData { get; set; }

    protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
    {
        if (config == "local")
        {
            optionsBuilder.UseInMemoryDatabase("InfoTrackDb");
        }
        else
        {
            var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connString))
            {
                throw new Exception("Database connection string is not set");
            }

            optionsBuilder.UseSqlServer(connString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackedSearchTerms>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<TrackedSearchTerms>()
            .HasOne(t => t.Url)
            .WithMany(u => u.SearchTerms)
            .HasForeignKey(t => t.UrlId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TrackedUrls>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<TrackedUrls>()
            .HasMany(u => u.SearchTerms)
            .WithOne(t => t.Url)
            .HasForeignKey(t => t.UrlId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TrackedSearchData>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<TrackedSearchData>()
            .HasOne(d => d.Url)
            .WithMany(u => u.SearchData)
            .HasForeignKey(d => d.UrlId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TrackedSearchData>()
            .HasOne(d => d.SearchTerms)
            .WithMany(u => u.SearchData)
            .HasForeignKey(d => d.TermId)
            .OnDelete(DeleteBehavior.NoAction);

        #region Seed the development database

        if (config == "local")
        {
            var seedData = SeedProvider.GetSeedData();
            modelBuilder.Entity<TrackedUrls>().HasData(seedData.Urls);
            modelBuilder.Entity<TrackedSearchTerms>().HasData(seedData.Terms);
            modelBuilder.Entity<TrackedSearchData>().HasData(seedData.SearchData);
        }

        #endregion
    }
}