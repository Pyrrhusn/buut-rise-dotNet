using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBatteryUsageTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "Battery",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "Battery",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "Battery");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "Battery");
        }
    }
}
