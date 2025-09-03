using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixFullNameLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "User",
                type: "character varying(202)",
                maxLength: 202,
                nullable: false,
                computedColumnSql: "\"FamilyName\" || ', ' || \"FirstName\"",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldComputedColumnSql: "\"FamilyName\" || ', ' || \"FirstName\"",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "User",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                computedColumnSql: "\"FamilyName\" || ', ' || \"FirstName\"",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(202)",
                oldMaxLength: 202,
                oldComputedColumnSql: "\"FamilyName\" || ', ' || \"FirstName\"",
                oldStored: true);
        }
    }
}
