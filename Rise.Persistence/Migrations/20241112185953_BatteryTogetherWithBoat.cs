using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BatteryTogetherWithBoat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Battery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BoatId = table.Column<int>(type: "integer", nullable: false),
                    MentorId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battery_Boat_BoatId",
                        column: x => x.BoatId,
                        principalTable: "Boat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Battery_User_MentorId",
                        column: x => x.MentorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Battery_BoatId",
                table: "Battery",
                column: "BoatId");

            migrationBuilder.CreateIndex(
                name: "IX_Battery_MentorId",
                table: "Battery",
                column: "MentorId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Battery");
        }
    }
}
