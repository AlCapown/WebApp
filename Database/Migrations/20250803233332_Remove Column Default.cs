using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Ended",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true,
                oldDefaultValueSql: "SYSDATETIMEOFFSET()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Ended",
                table: "BackgroundJobLog",
                type: "DATETIMEOFFSET(0)",
                nullable: true,
                defaultValueSql: "SYSDATETIMEOFFSET()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "DATETIMEOFFSET(0)",
                oldNullable: true);
        }
    }
}
