using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class Addbackgroundjobandinvitecodetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackgroundJobLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackgroundJobName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Started = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(0)", nullable: true, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    Ended = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(0)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundJobLog", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "InviteCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(0)", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    Expires = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteCode", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundJobLog");

            migrationBuilder.DropTable(
                name: "InviteCode");
        }
    }
}
