using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCruisePeriodAndTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Product",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.CreateTable(
                name: "CruisePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CruisePeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CruisePeriodId = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<TimeSpan>(type: "interval", nullable: false),
                    End = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlots_CruisePeriods_CruisePeriodId",
                        column: x => x.CruisePeriodId,
                        principalTable: "CruisePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_CruisePeriodId",
                table: "TimeSlots",
                column: "CruisePeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "CruisePeriods");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Product",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "NOW()");
        }
    }
}
