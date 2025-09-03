using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompositeKeyTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlot_CruisePeriod_CruisePeriodId",
                table: "TimeSlot");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TimeSlot_Date_Start_End",
                table: "TimeSlot");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlot_CruisePeriod_CruisePeriodId",
                table: "TimeSlot",
                column: "CruisePeriodId",
                principalTable: "CruisePeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlot_CruisePeriod_CruisePeriodId",
                table: "TimeSlot");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TimeSlot_Date_Start_End",
                table: "TimeSlot",
                columns: new[] { "Date", "Start", "End" });



        }
    }
}
