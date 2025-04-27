using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace WebApp.Database.Migrations;

/// <inheritdoc />
public partial class AddsGamePredictionTablev2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DateCreated",
            table: "GamePrediction",
            type: "DATETIMEOFFSET(0)",
            nullable: false,
            defaultValueSql: "SYSDATETIMEOFFSET()");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DateCreated",
            table: "GamePrediction");
    }
}
