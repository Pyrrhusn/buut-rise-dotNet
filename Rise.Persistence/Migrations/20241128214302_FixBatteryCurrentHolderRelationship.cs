using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBatteryCurrentHolderRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Battery_User_CurrentHolderId1",
                table: "Battery");

            migrationBuilder.RenameColumn(
                name: "CurrentHolderId1",
                table: "Battery",
                newName: "CurrentHolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Battery_CurrentHolderId1",
                table: "Battery",
                newName: "IX_Battery_CurrentHolderId");

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

            migrationBuilder.RenameColumn(
                name: "CurrentHolderId",
                table: "Battery",
                newName: "CurrentHolderId1");

            migrationBuilder.RenameIndex(
                name: "IX_Battery_CurrentHolderId",
                table: "Battery",
                newName: "IX_Battery_CurrentHolderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Battery_User_CurrentHolderId1",
                table: "Battery",
                column: "CurrentHolderId1",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
