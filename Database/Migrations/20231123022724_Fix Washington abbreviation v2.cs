using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixWashingtonabbreviationv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 16,
                column: "Abbreviation",
                value: "WSH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 16,
                column: "Abbreviation",
                value: "WHS");
        }
    }
}
