using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDMarketData.BackTesting.Data.Helpers;
using TDMarketData.BackTesting.Data.Models;

namespace TDMarketData.BackTesting.Data
{
    public class BackTestContext : DbContext
    {

        //public static readonly ILoggerFactory MyLoggerFactory
        // = LoggerFactory.Create(builder => { builder.AddDebug(); });


        public DbSet<Position> Positions { get; set; }
        public DbSet<OptionContract> OptionContracts { get; set; }
        public DbSet<OptionCandle> OptionCandles { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<OptionTimeSale> OptionTimeSales { get; set; }
        public DbSet<OptionSymbolDailyStat> DailyStats { get; set; }
        public DbSet<OptionSymbolDailyStatDetail> DailyStatDetails { get; set; }
        public DbSet<FileStatus> FileStatus { get; set; }
        public DbSet<LargeTradeSummary> LargeTrades {get; set; }

        public BackTestContext()
        {
            Database.SetCommandTimeout(120);
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            //var connection = Database.GetDbConnection();
            //connection.Open();
            
            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = "PRAGMA SYNCHRONOUS=0;";
            //    command.ExecuteNonQuery();
            //}
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLoggerFactory(MyLoggerFactory).EnableSensitiveDataLogging(); // Warning: Do not create a new ILoggerFactory instance each time
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;" +
                    "User Id=postgres;Password=3Ofspades!;Database=MARKETDATA;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnderlyingData>().HasNoKey();

            modelBuilder.Entity<OptionCandle>()
                .HasOne(p => p.OptionContract)
                .WithMany(b => b.OptionCandles);

            modelBuilder.Entity<OptionTimeSale>()
                .HasOne(o => o.OptionContract);


            modelBuilder.Entity<OptionCandle>().Ignore(o => o.Symbol);
            modelBuilder.Entity<OptionTimeSale>().Ignore(o => o.Symbol);

            modelBuilder.Entity<Position>()
                .HasOne(o => o.OptionContract);

            modelBuilder.Entity<Position>().Ignore(p => p.OptionCandles);

            modelBuilder.Entity<Position>()
                .HasOne(o => o.Porfolio)
                .WithMany(p => p.Positions);

            modelBuilder.Entity<OptionSymbolDailyStatDetail>()
                .HasOne(o => o.DailyStat).WithOne(o => o.StatDetail).HasForeignKey<OptionSymbolDailyStatDetail>(o => o.OptionSymbolDailyStatId );

            modelBuilder.Entity<FileStatus>();

        }
    }
}
