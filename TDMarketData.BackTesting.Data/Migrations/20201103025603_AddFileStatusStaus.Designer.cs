﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TDMarketData.BackTesting.Data;

namespace TDMarketData.BackTesting.Data.Migrations
{
    [DbContext(typeof(BackTestContext))]
    [Migration("20201103025603_AddFileStatusStaus")]
    partial class AddFileStatusStaus
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Helpers.UnderlyingData", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("UnderlyingPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("UnderlyingSymbol")
                        .HasColumnType("text");

                    b.Property<string>("Volatility")
                        .HasColumnType("text");

                    b.ToTable("UnderlyingData");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.FileStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<DateTime>("ProcessedDt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FileStatus");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionCandle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Ask")
                        .HasColumnType("double precision");

                    b.Property<double>("Bid")
                        .HasColumnType("double precision");

                    b.Property<string>("BidAskSize")
                        .HasColumnType("text");

                    b.Property<double>("Close")
                        .HasColumnType("double precision");

                    b.Property<long>("Datetime")
                        .HasColumnType("bigint");

                    b.Property<long>("OpenInterest")
                        .HasColumnType("bigint");

                    b.Property<int>("OptionContractId")
                        .HasColumnType("integer");

                    b.Property<double>("PercentChange")
                        .HasColumnType("double precision");

                    b.Property<double>("UnderlyingPrice")
                        .HasColumnType("double precision");

                    b.Property<double>("Volatility")
                        .HasColumnType("double precision");

                    b.Property<long>("Volume")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("OptionContractId");

                    b.ToTable("OptionCandles");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionContract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<char>("CallPut")
                        .HasColumnType("character(1)");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Strike")
                        .HasColumnType("double precision");

                    b.Property<string>("Symbol")
                        .HasColumnType("text");

                    b.Property<string>("UnderlyingSymbol")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OptionContracts");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionSymbolDailyStat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("CallNotionalValue")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("PutNotionalValue")
                        .HasColumnType("bigint");

                    b.Property<string>("callSizIdx")
                        .HasColumnType("text");

                    b.Property<string>("callVol")
                        .HasColumnType("text");

                    b.Property<string>("hv52High")
                        .HasColumnType("text");

                    b.Property<string>("hv52Low")
                        .HasColumnType("text");

                    b.Property<string>("iv")
                        .HasColumnType("text");

                    b.Property<string>("iv52High")
                        .HasColumnType("text");

                    b.Property<string>("iv52Low")
                        .HasColumnType("text");

                    b.Property<string>("percHV")
                        .HasColumnType("text");

                    b.Property<string>("percIV")
                        .HasColumnType("text");

                    b.Property<string>("putSizIdx")
                        .HasColumnType("text");

                    b.Property<string>("putVol")
                        .HasColumnType("text");

                    b.Property<string>("sizIdx")
                        .HasColumnType("text");

                    b.Property<string>("stSizIdx")
                        .HasColumnType("text");

                    b.Property<string>("symbol")
                        .HasColumnType("text");

                    b.Property<string>("totalVol")
                        .HasColumnType("text");

                    b.Property<string>("volSizIdx")
                        .HasColumnType("text");

                    b.Property<string>("vwap")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DailyStats");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionSymbolDailyStatDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("NotionalValue180DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValue20DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValue30DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValue5DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc10")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc20")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc30")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc40")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc50")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc60")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc70")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc80")
                        .HasColumnType("double precision");

                    b.Property<double>("NotionalValueXLPerc90")
                        .HasColumnType("double precision");

                    b.Property<int>("OptionSymbolDailyStatId")
                        .HasColumnType("integer");

                    b.Property<double>("PutCallRatio180DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("PutCallRatio20DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("PutCallRatio30DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("PutCallRatio5DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volatility180DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volatility20DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volatility30DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volatility5DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volume180DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volume20DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volume30DayMvgAvg")
                        .HasColumnType("double precision");

                    b.Property<double>("Volume5DayMvgAvg")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("OptionSymbolDailyStatId")
                        .IsUnique();

                    b.ToTable("DailyStatDetails");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionTimeSale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Ask")
                        .HasColumnType("double precision");

                    b.Property<double>("Bid")
                        .HasColumnType("double precision");

                    b.Property<int>("OptionContractId")
                        .HasColumnType("integer");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<int>("Qty")
                        .HasColumnType("integer");

                    b.Property<long>("Time")
                        .HasColumnType("bigint");

                    b.Property<int>("TradeId")
                        .HasColumnType("integer");

                    b.Property<double>("UnderlyingPrice")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("OptionContractId");

                    b.ToTable("OptionTimeSales");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.Portfolio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Portfolio");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("OptionContractId")
                        .HasColumnType("integer");

                    b.Property<int>("PortfolioId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<DateTime>("TradeDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("TradeId")
                        .HasColumnType("integer");

                    b.Property<double>("TradePrice")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("OptionContractId");

                    b.HasIndex("PortfolioId");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionCandle", b =>
                {
                    b.HasOne("TDMarketData.BackTesting.Data.Models.OptionContract", "OptionContract")
                        .WithMany("OptionCandles")
                        .HasForeignKey("OptionContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionSymbolDailyStatDetail", b =>
                {
                    b.HasOne("TDMarketData.BackTesting.Data.Models.OptionSymbolDailyStat", "DailyStat")
                        .WithOne("StatDetail")
                        .HasForeignKey("TDMarketData.BackTesting.Data.Models.OptionSymbolDailyStatDetail", "OptionSymbolDailyStatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.OptionTimeSale", b =>
                {
                    b.HasOne("TDMarketData.BackTesting.Data.Models.OptionContract", "OptionContract")
                        .WithMany()
                        .HasForeignKey("OptionContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TDMarketData.BackTesting.Data.Models.Position", b =>
                {
                    b.HasOne("TDMarketData.BackTesting.Data.Models.OptionContract", "OptionContract")
                        .WithMany()
                        .HasForeignKey("OptionContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TDMarketData.BackTesting.Data.Models.Portfolio", "Porfolio")
                        .WithMany("Positions")
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
