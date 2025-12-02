using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedulee.Migrations
{
    /// <inheritdoc />
    public partial class BioAndAutonomosAtt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "MeiCadastros",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "MeiCadastros",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "MeiCadastros",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "MeiCadastros");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "MeiCadastros");

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "MeiCadastros",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
