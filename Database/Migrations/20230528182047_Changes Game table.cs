using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApp.Database.Migrations;

/// <inheritdoc />
public partial class ChangesGametable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "390eddd4-11b2-4af3-b10a-e84ae12084fd");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "a7f02fd6-8d5d-40bd-8446-c6440d0929ee");

        migrationBuilder.RenameColumn(
            name: "TeamShortName",
            table: "Team",
            newName: "Abbreviation");

        migrationBuilder.RenameIndex(
            name: "IX_Team_TeamShortName",
            table: "Team",
            newName: "IX_Team_Abbreviation");

        migrationBuilder.AlterColumn<int>(
            name: "Quarter",
            table: "Game",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ClockTime",
            table: "Game",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "6fa34abf-9fe9-4871-b266-e3ef928569da", null, "AppIdentityRole", "USER", "USER" },
                { "7b94084d-41e2-48f9-9b2d-50f5adcdecb6", null, "AppIdentityRole", "ADMIN", "ADMIN" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "6fa34abf-9fe9-4871-b266-e3ef928569da");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "7b94084d-41e2-48f9-9b2d-50f5adcdecb6");

        migrationBuilder.RenameColumn(
            name: "Abbreviation",
            table: "Team",
            newName: "TeamShortName");

        migrationBuilder.RenameIndex(
            name: "IX_Team_Abbreviation",
            table: "Team",
            newName: "IX_Team_TeamShortName");

        migrationBuilder.AlterColumn<string>(
            name: "Quarter",
            table: "Game",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "ClockTime",
            table: "Game",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "390eddd4-11b2-4af3-b10a-e84ae12084fd", null, "AppIdentityRole", "USER", "USER" },
                { "a7f02fd6-8d5d-40bd-8446-c6440d0929ee", null, "AppIdentityRole", "ADMIN", "ADMIN" }
            });
    }
}
