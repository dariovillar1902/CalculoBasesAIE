using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoNuevasVariablesCuantia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CargaMayorada1",
                table: "BasesHormigonCuantias",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CargaMayorada2",
                table: "BasesHormigonCuantias",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ExcentricidadMayorada",
                table: "BasesHormigonCuantias",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MomentoMayorado",
                table: "BasesHormigonCuantias",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CargaMayorada1",
                table: "BasesHormigonCuantias");

            migrationBuilder.DropColumn(
                name: "CargaMayorada2",
                table: "BasesHormigonCuantias");

            migrationBuilder.DropColumn(
                name: "ExcentricidadMayorada",
                table: "BasesHormigonCuantias");

            migrationBuilder.DropColumn(
                name: "MomentoMayorado",
                table: "BasesHormigonCuantias");
        }
    }
}
