using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class DotNet10Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ADMIN",
                column: "ConcurrencyStamp",
                value: "ADB1B49F-9F45-4849-B7E0-020DFA85DCFA");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "USER",
                column: "ConcurrencyStamp",
                value: "32AE3B78-8A7A-4093-9423-B5F1B253F311");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ADMIN",
                column: "ConcurrencyStamp",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "USER",
                column: "ConcurrencyStamp",
                value: null);
        }
    }
}
