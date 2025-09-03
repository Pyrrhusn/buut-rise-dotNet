using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FamilyName",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(65)",
                oldMaxLength: 65);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "User",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Number",
                table: "User",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "User",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notification",
                type: "character varying(130)",
                maxLength: 130,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Address_Number",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "FamilyName",
                table: "User",
                type: "character varying(65)",
                maxLength: 65,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notification",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(130)",
                oldMaxLength: 130);
        }
    }
}
