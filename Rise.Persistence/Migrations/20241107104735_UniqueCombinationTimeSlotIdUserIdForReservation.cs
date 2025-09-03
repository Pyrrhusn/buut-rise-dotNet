using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueCombinationTimeSlotIdUserIdForReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservation_UserId",
                table: "Reservation");

            migrationBuilder.CreateIndex(
                name: "IX_Unique_User_TimeSlot",
                table: "Reservation",
                columns: new[] { "UserId", "TimeSlotId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unique_User_TimeSlot",
                table: "Reservation");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_UserId",
                table: "Reservation",
                column: "UserId");
        }
    }
}
