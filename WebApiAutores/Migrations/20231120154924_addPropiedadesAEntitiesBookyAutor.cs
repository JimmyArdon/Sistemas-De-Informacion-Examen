using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    /// <inheritdoc />
    public partial class addPropiedadesAEntitiesBookyAutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url_imagen",
                schema: "transacctional",
                table: "books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url_imagen",
                schema: "transacctional",
                table: "autores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url_imagen",
                schema: "transacctional",
                table: "books");

            migrationBuilder.DropColumn(
                name: "url_imagen",
                schema: "transacctional",
                table: "autores");
        }
    }
}
