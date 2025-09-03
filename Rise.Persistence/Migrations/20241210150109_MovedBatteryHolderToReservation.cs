using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovedBatteryHolderToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Battery_CurrentHolderId",
                table: "Battery");

            migrationBuilder.DropForeignKey(
                name: "FK_Battery_User_CurrentHolderId",
                table: "Battery");

            migrationBuilder.DropColumn(
                name: "CurrentHolderId",
                table: "Battery");

            migrationBuilder.AddColumn<int>(
                name: "PreviousBatteryHolderId",
                table: "Reservation",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_PreviousBatteryHolderId",
                table: "Reservation",
                column: "PreviousBatteryHolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_User_PreviousBatteryHolderId",
                table: "Reservation",
                column: "PreviousBatteryHolderId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_User_PreviousBatteryHolderId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_PreviousBatteryHolderId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "PreviousBatteryHolderId",
                table: "Reservation");

            migrationBuilder.AddColumn<int>(
            name: "CurrentHolderId",
            table: "Battery",
            type: "integer",
            nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battery_CurrentHolderId",
                table: "Battery",
                column: "CurrentHolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Battery_User_CurrentHolderId",
                table: "Battery",
                column: "CurrentHolderId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
