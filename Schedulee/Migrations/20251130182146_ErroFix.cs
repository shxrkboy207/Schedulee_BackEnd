using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedulee.Migrations
{
    /// <inheritdoc />
    public partial class ErroFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoConta",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoConta",
                table: "Usuarios");
        }
    }
}
