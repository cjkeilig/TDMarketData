using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TDMarketData.BackTesting.Data.Migrations
{
    public partial class PositionSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "LargeTrades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<long>(nullable: false),
                    OptionContractId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Bid = table.Column<double>(nullable: false),
                    Ask = table.Column<double>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    NotionalValue = table.Column<double>(nullable: false),
                    OpenInterestChange = table.Column<long>(nullable: false),
                    OpenedClosed = table.Column<string>(nullable: true),
                    OptionTimeSaleId = table.Column<int>(nullable: false),
                    LargeTradeSummaryParentTradeId = table.Column<int>(nullable: false),
                    IsParentTrade = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LargeTrades", x => x.Id);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LargeTrades");

        }
    }
}
