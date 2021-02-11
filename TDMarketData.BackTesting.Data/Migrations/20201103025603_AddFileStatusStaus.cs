using Microsoft.EntityFrameworkCore.Migrations;

namespace TDMarketData.BackTesting.Data.Migrations
{
    public partial class AddFileStatusStaus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FileStatus",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "FileStatus");
        }
    }
}
