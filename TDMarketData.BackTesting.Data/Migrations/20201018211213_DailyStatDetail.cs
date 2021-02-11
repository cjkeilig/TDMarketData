using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TDMarketData.BackTesting.Data.Migrations
{
    public partial class DailyStatDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "DailyStatDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OptionSymbolDailyStatId = table.Column<int>(nullable: false),
                    PutCallRatio5DayMvgAvg = table.Column<double>(nullable: false),
                    PutCallRatio20DayMvgAvg = table.Column<double>(nullable: false),
                    PutCallRatio30DayMvgAvg = table.Column<double>(nullable: false),
                    PutCallRatio180DayMvgAvg = table.Column<double>(nullable: false),
                    Volume5DayMvgAvg = table.Column<double>(nullable: false),
                    Volume20DayMvgAvg = table.Column<double>(nullable: false),
                    Volume30DayMvgAvg = table.Column<double>(nullable: false),
                    Volume180DayMvgAvg = table.Column<double>(nullable: false),
                    Volatility5DayMvgAvg = table.Column<double>(nullable: false),
                    Volatility20DayMvgAvg = table.Column<double>(nullable: false),
                    Volatility30DayMvgAvg = table.Column<double>(nullable: false),
                    Volatility180DayMvgAvg = table.Column<double>(nullable: false),
                    NotionalValue5DayMvgAvg = table.Column<double>(nullable: false),
                    NotionalValue20DayMvgAvg = table.Column<double>(nullable: false),
                    NotionalValue30DayMvgAvg = table.Column<double>(nullable: false),
                    NotionalValue180DayMvgAvg = table.Column<double>(nullable: false),
                    NotionalValueXLPerc10 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc20 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc30 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc40 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc50 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc60 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc70 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc80 = table.Column<double>(nullable: false),
                    NotionalValueXLPerc90 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyStatDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyStatDetails_DailyStats_OptionSymbolDailyStatId",
                        column: x => x.OptionSymbolDailyStatId,
                        principalTable: "DailyStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyStatDetails_OptionSymbolDailyStatId",
                table: "DailyStatDetails",
                column: "OptionSymbolDailyStatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyStatDetails");

        }
    }
}
