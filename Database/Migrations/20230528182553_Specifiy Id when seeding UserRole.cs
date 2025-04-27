using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApp.Database.Migrations;

/// <inheritdoc />
public partial class SpecifiyIdwhenseedingUserRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "6fa34abf-9fe9-4871-b266-e3ef928569da");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "7b94084d-41e2-48f9-9b2d-50f5adcdecb6");

        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "ADMIN", null, "AppIdentityRole", "ADMIN", "ADMIN" },
                { "USER", null, "AppIdentityRole", "USER", "USER" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "ADMIN");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "USER");

        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "6fa34abf-9fe9-4871-b266-e3ef928569da", null, "AppIdentityRole", "USER", "USER" },
                { "7b94084d-41e2-48f9-9b2d-50f5adcdecb6", null, "AppIdentityRole", "ADMIN", "ADMIN" }
            });
    }
}
