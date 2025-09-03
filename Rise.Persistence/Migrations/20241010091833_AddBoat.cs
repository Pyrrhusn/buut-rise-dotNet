using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "TimeSlots",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "Boat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    PersonalName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boat", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boat");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "TimeSlots");
        }
    }
}
