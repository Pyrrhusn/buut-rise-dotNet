using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueCombinationTimeSlotIdBoatIdForReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservation_BoatId",
                table: "Reservation");

            migrationBuilder.CreateIndex(
                name: "IX_Unique_Boat_TimeSlot",
                table: "Reservation",
                columns: new[] { "BoatId", "TimeSlotId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unique_Boat_TimeSlot",
                table: "Reservation");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_BoatId",
                table: "Reservation",
                column: "BoatId");
        }
    }
}
