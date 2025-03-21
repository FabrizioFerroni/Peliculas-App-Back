using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasApp_Back.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToPeliculaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pesonaje",
                table: "PeliculaActores",
                newName: "Personaje");

            migrationBuilder.AddColumn<string>(
                name: "AnioLanzamiento",
                table: "Peliculas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Peliculas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duracion",
                table: "Peliculas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnCines",
                table: "Peliculas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProximoEstreno",
                table: "Peliculas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnioLanzamiento",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "EnCines",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "ProximoEstreno",
                table: "Peliculas");

            migrationBuilder.RenameColumn(
                name: "Personaje",
                table: "PeliculaActores",
                newName: "Pesonaje");
        }
    }
}
