using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBatteryUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Battery_User_CurrentHolderId",
                table: "Battery");

            migrationBuilder.DropIndex(
                name: "IX_Battery_CurrentHolderId",
                table: "Battery");

            migrationBuilder.DropColumn(
                name: "CurrentHolderId",
                table: "Battery");
        }
    }
}
