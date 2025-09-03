using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBatteryIdToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatteryId",
                table: "Reservation",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_BatteryId",
                table: "Reservation",
                column: "BatteryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Battery_BatteryId",
                table: "Reservation",
                column: "BatteryId",
                principalTable: "Battery",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Battery_BatteryId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_BatteryId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "BatteryId",
                table: "Reservation");
        }
    }
}
