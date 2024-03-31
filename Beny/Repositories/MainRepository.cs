using Beny.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Beny.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasIndex(p => p.Name).IsUnique();
        }
    }
}
