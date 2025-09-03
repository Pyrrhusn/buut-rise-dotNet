using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RelationReservationAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationUser");
        }
    }
}
