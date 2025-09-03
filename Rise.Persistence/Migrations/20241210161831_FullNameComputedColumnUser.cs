using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FullNameComputedColumnUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "User",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                computedColumnSql: "\"FamilyName\" || ', ' || \"FirstName\"",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "User");
        }
    }
}
