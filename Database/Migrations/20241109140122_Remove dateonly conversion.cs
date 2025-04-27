using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class Removedateonlyconversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Season",
                keyColumn: "SeasonId",
                keyValue: 2022,
                columns: new[] { "SeasonEnd", "SeasonStart" },
                values: new object[] { new DateOnly(1, 1, 1), new DateOnly(1, 1, 1) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Season",
                keyColumn: "SeasonId",
                keyValue: 2022,
                columns: new[] { "SeasonEnd", "SeasonStart" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
