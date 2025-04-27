using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations;

/// <inheritdoc />
public partial class AddsGamePredictionTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "FootballGameId",
            table: "Game",
            newName: "GameId");

        migrationBuilder.CreateTable(
            name: "GamePrediction",
            columns: table => new
            {
                GamePredictionId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GameId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                HomeTeamScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                AwayTeamScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GamePrediction", x => x.GamePredictionId);
                table.ForeignKey(
                    name: "FK_GamePrediction_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_GamePrediction_Game_GameId",
                    column: x => x.GameId,
                    principalTable: "Game",
                    principalColumn: "GameId",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GamePrediction_GameId_UserId",
            table: "GamePrediction",
            columns: new[] { "GameId", "UserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_GamePrediction_UserId",
            table: "GamePrediction",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GamePrediction");

        migrationBuilder.RenameColumn(
            name: "GameId",
            table: "Game",
            newName: "FootballGameId");
    }
}
