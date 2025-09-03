using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class compositeKeyForTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_TimeSlots_Date_Start_End",
                table: "TimeSlots",
                columns: new[] { "Date", "Start", "End" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TimeSlots_Date_Start_End",
                table: "TimeSlots");
        }
    }
}
