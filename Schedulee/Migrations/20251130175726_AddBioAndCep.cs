using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedulee.Migrations
{
    /// <inheritdoc />
    public partial class AddBioAndCep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Usuarios");
        }
    }
}
