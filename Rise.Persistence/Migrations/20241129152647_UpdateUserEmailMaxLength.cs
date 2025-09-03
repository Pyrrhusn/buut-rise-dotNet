using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserEmailMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "User",
                newName: "IX_Unique_User_Email");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "character varying(69)",
                maxLength: 69,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Unique_User_Email",
                table: "User",
                newName: "IX_User_Email");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(69)",
                oldMaxLength: 69);
        }
    }
}
