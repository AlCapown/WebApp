using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixRamsabbreviation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 8,
                column: "Abbreviation",
                value: "LAR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 8,
                column: "Abbreviation",
                value: "LA");
        }
    }
}
