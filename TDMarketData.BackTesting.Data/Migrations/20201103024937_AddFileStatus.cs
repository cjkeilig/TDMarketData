using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TDMarketData.BackTesting.Data.Migrations
{
    public partial class AddFileStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyStatDetails_OptionSymbolDailyStatId",
                table: "DailyStatDetails");

            migrationBuilder.CreateIndex(
                name: "IX_DailyStatDetails_OptionSymbolDailyStatId",
                table: "DailyStatDetails",
                column: "OptionSymbolDailyStatId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyStatDetails_OptionSymbolDailyStatId",
                table: "DailyStatDetails");

            migrationBuilder.CreateIndex(
                name: "IX_DailyStatDetails_OptionSymbolDailyStatId",
                table: "DailyStatDetails",
                column: "OptionSymbolDailyStatId");
        }
    }
}
