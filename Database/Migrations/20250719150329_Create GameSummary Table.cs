using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateGameSummaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSummary",
                columns: table => new
                {
                    GameAISummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSummary", x => x.GameAISummaryId);
                    table.ForeignKey(
                        name: "FK_GameSummary_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSummary_GameId",
                table: "GameSummary",
                column: "GameId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSummary");
        }
    }
}
