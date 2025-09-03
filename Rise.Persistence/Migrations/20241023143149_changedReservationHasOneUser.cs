using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changedReservationHasOneUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationUser");

            migrationBuilder.DropColumn(
                name: "AmountAdults",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "AmountChildren",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "AmountPets",
                table: "Reservation",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_UserId",
                table: "Reservation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_User_UserId",
                table: "Reservation",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_User_UserId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_UserId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservation",
                newName: "AmountPets");

            migrationBuilder.AddColumn<int>(
                name: "AmountAdults",
                table: "Reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmountChildren",
                table: "Reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReservationUser",
                columns: table => new
                {
                    ReservationsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationUser", x => new { x.ReservationsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ReservationUser_Reservation_ReservationsId",
                        column: x => x.ReservationsId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationUser_UsersId",
                table: "ReservationUser",
                column: "UsersId");
        }
    }
}
