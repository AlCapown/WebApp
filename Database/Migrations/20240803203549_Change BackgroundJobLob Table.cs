using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBackgroundJobLobTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "BackgroundJobLog");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Started",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true,
                oldDefaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Ended",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                defaultValueSql: "SYSDATETIMEOFFSET()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorsJson",
                table: "BackgroundJobLog",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "BackgroundJobLog",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorsJson",
                table: "BackgroundJobLog");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "BackgroundJobLog");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Started",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                defaultValueSql: "SYSDATETIMEOFFSET()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Ended",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true,
                oldDefaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "BackgroundJobLog",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);
        }
    }
}
