using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasApp_Back.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Generos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Actores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Generos");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Actores");
        }
    }
}
