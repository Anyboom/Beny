using Beny.Models;
using Microsoft.EntityFrameworkCore;

namespace Beny.Repositories
{
    public class MainRepository : DbContext
    {
        public DbSet<Bet> Bets { get; set; } = null!;
        public DbSet<FootballEvent> FootballEvents { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Competition> Competitions { get; set; } = null!;
        public DbSet<Sport> Sports { get; set; } = null!;
        public DbSet<Forecast> Forecasts { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<FootballEventTag> FootballEventTag { get; set; } = null!;

        public MainRepository()
        {
            Database.EnsureCreatedAsync();

            Bets.LoadAsync();
            FootballEvents.LoadAsync();

            Teams.LoadAsync();
            Forecasts.LoadAsync();
            Sports.LoadAsync();
            Competitions.LoadAsync();
            FootballEventTag.LoadAsync();
            Tags.LoadAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Beny.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Competition>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Sport>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Forecast>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<FootballEvent>().Property(x => x.IsLive).HasDefaultValue(false);

            modelBuilder.Entity<FootballEventTag>()
                .HasKey(bc => new { bc.FootballEventId, bc.TagId });

            modelBuilder.Entity<FootballEventTag>()
                .HasOne(bc => bc.FootballEvent)
                .WithMany(b => b.FootballEventTags)
                .HasForeignKey(bc => bc.FootballEventId);

            modelBuilder.Entity<FootballEventTag>()
                .HasOne(bc => bc.Tag)
                .WithMany(c => c.FootballEventTags)
                .HasForeignKey(bc => bc.TagId);
        }
    }
}
