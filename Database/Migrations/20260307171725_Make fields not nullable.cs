using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class Makefieldsnotnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartsOn",
                table: "Game",
                type: "DATETIMEOFFSET(0)",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true,
                oldDefaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Started",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Ended",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartsOn",
                table: "Game",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                defaultValueSql: "SYSDATETIMEOFFSET()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldDefaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Started",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Ended",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)");
        }
    }
}
