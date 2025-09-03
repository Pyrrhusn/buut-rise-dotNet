using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BatteryResponsiblitiesForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Battery_MentorId",
                table: "Battery");

            migrationBuilder.CreateIndex(
                name: "IX_Battery_MentorId",
                table: "Battery",
                column: "MentorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Battery_MentorId",
                table: "Battery");

            migrationBuilder.CreateIndex(
                name: "IX_Battery_MentorId",
                table: "Battery",
                column: "MentorId",
                unique: true);
        }
    }
}
