using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });
        }
    }
}
