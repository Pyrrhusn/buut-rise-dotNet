using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RelationReservationAndTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "Reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_TimeSlotId",
                table: "Reservation",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_TimeSlots_TimeSlotId",
                table: "Reservation",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_TimeSlots_TimeSlotId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_TimeSlotId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Reservation");
        }
    }
}
