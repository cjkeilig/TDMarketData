using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TDMarketData.BackTesting.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    symbol = table.Column<string>(nullable: true),
                    totalVol = table.Column<string>(nullable: true),
                    putVol = table.Column<string>(nullable: true),
                    callVol = table.Column<string>(nullable: true),
                    iv = table.Column<string>(nullable: true),
                    vwap = table.Column<string>(nullable: true),
                    iv52High = table.Column<string>(nullable: true),
                    iv52Low = table.Column<string>(nullable: true),
                    percIV = table.Column<string>(nullable: true),
                    hv52High = table.Column<string>(nullable: true),
                    hv52Low = table.Column<string>(nullable: true),
                    percHV = table.Column<string>(nullable: true),
                    sizIdx = table.Column<string>(nullable: true),
                    callSizIdx = table.Column<string>(nullable: true),
                    putSizIdx = table.Column<string>(nullable: true),
                    volSizIdx = table.Column<string>(nullable: true),
                    stSizIdx = table.Column<string>(nullable: true),
                    CallNotionalValue = table.Column<long>(nullable: false),
                    PutNotionalValue = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionContracts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(nullable: true),
                    UnderlyingSymbol = table.Column<string>(nullable: true),
                    Strike = table.Column<double>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    CallPut = table.Column<char>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionContracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portfolio",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnderlyingSymbolPrice",
                columns: table => new
                {
                    UnderlyingSymbol = table.Column<string>(nullable: true),
                    UnderlyingPrice = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "OptionCandles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OptionContractId = table.Column<int>(nullable: false),
                    Close = table.Column<double>(nullable: false),
                    Volume = table.Column<long>(nullable: false),
                    Datetime = table.Column<long>(nullable: false),
                    Volatility = table.Column<double>(nullable: false),
                    OpenInterest = table.Column<long>(nullable: false),
                    PercentChange = table.Column<double>(nullable: false),
                    BidAskSize = table.Column<string>(nullable: true),
                    Bid = table.Column<double>(nullable: false),
                    Ask = table.Column<double>(nullable: false),
                    UnderlyingPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionCandles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionCandles_OptionContracts_OptionContractId",
                        column: x => x.OptionContractId,
                        principalTable: "OptionContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionTimeSales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OptionContractId = table.Column<int>(nullable: false),
                    Time = table.Column<long>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    Bid = table.Column<double>(nullable: false),
                    Ask = table.Column<double>(nullable: false),
                    UnderlyingPrice = table.Column<double>(nullable: false),
                    TradeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionTimeSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionTimeSales_OptionContracts_OptionContractId",
                        column: x => x.OptionContractId,
                        principalTable: "OptionContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PortfolioId = table.Column<int>(nullable: false),
                    OptionContractId = table.Column<int>(nullable: false),
                    TradePrice = table.Column<double>(nullable: false),
                    TradeDate = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TradeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_OptionContracts_OptionContractId",
                        column: x => x.OptionContractId,
                        principalTable: "OptionContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Positions_Portfolio_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionCandles_OptionContractId",
                table: "OptionCandles",
                column: "OptionContractId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTimeSales_OptionContractId",
                table: "OptionTimeSales",
                column: "OptionContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_OptionContractId",
                table: "Positions",
                column: "OptionContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PortfolioId",
                table: "Positions",
                column: "PortfolioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyStats");

            migrationBuilder.DropTable(
                name: "OptionCandles");

            migrationBuilder.DropTable(
                name: "OptionTimeSales");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "UnderlyingSymbolPrice");

            migrationBuilder.DropTable(
                name: "OptionContracts");

            migrationBuilder.DropTable(
                name: "Portfolio");
        }
    }
}
