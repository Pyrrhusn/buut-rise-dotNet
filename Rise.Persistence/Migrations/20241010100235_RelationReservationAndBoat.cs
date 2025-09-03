using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RelationReservationAndBoat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoatId",
                table: "Reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_BoatId",
                table: "Reservation",
                column: "BoatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Boat_BoatId",
                table: "Reservation",
                column: "BoatId",
                principalTable: "Boat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Boat_BoatId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_BoatId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "BoatId",
                table: "Reservation");
        }
    }
}
