using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changedTableNameTimeSlotAndCruisePeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_TimeSlots_TimeSlotId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_CruisePeriods_CruisePeriodId",
                table: "TimeSlots");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TimeSlots_Date_Start_End",
                table: "TimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSlots",
                table: "TimeSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CruisePeriods",
                table: "CruisePeriods");

            migrationBuilder.RenameTable(
                name: "TimeSlots",
                newName: "TimeSlot");

            migrationBuilder.RenameTable(
                name: "CruisePeriods",
                newName: "CruisePeriod");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSlots_CruisePeriodId",
                table: "TimeSlot",
                newName: "IX_TimeSlot_CruisePeriodId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TimeSlot_Date_Start_End",
                table: "TimeSlot",
                columns: new[] { "Date", "Start", "End" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSlot",
                table: "TimeSlot",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CruisePeriod",
                table: "CruisePeriod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_TimeSlot_TimeSlotId",
                table: "Reservation",
                column: "TimeSlotId",
                principalTable: "TimeSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Reservation_TimeSlot_TimeSlotId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlot_CruisePeriod_CruisePeriodId",
                table: "TimeSlot");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TimeSlot_Date_Start_End",
                table: "TimeSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSlot",
                table: "TimeSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CruisePeriod",
                table: "CruisePeriod");

            migrationBuilder.RenameTable(
                name: "TimeSlot",
                newName: "TimeSlots");

            migrationBuilder.RenameTable(
                name: "CruisePeriod",
                newName: "CruisePeriods");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSlot_CruisePeriodId",
                table: "TimeSlots",
                newName: "IX_TimeSlots_CruisePeriodId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TimeSlots_Date_Start_End",
                table: "TimeSlots",
                columns: new[] { "Date", "Start", "End" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSlots",
                table: "TimeSlots",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CruisePeriods",
                table: "CruisePeriods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_TimeSlots_TimeSlotId",
                table: "Reservation",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_CruisePeriods_CruisePeriodId",
                table: "TimeSlots",
                column: "CruisePeriodId",
                principalTable: "CruisePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
