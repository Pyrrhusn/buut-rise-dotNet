using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedLastUsedForBattery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "Battery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "Battery",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
