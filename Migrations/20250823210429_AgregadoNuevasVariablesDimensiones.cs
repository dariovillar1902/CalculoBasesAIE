using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoNuevasVariablesDimensiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AreaNecesaria",
                table: "BasesHormigonDimensiones",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CargaDiseno",
                table: "BasesHormigonDimensiones",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RelacionLados",
                table: "BasesHormigonDimensiones",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TensionPromedio",
                table: "BasesHormigonDimensiones",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "EsfuerzoCorteX_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EsfuerzoCorteX_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "EsfuerzoCorteX_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "EsfuerzoCorteY_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EsfuerzoCorteY_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "EsfuerzoCorteY_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ModuloBalastoVertical_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModuloBalastoVertical_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ModuloBalastoVertical_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MomentoX_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MomentoX_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "MomentoX_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MomentoY_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MomentoY_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "MomentoY_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TensionX = table.Column<double>(type: "float", nullable: false),
                    TensionY = table.Column<double>(type: "float", nullable: false),
                    VerificaTension = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonVerificaciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasesHormigonVerificaciones");

            migrationBuilder.DropColumn(
                name: "AreaNecesaria",
                table: "BasesHormigonDimensiones");

            migrationBuilder.DropColumn(
                name: "CargaDiseno",
                table: "BasesHormigonDimensiones");

            migrationBuilder.DropColumn(
                name: "RelacionLados",
                table: "BasesHormigonDimensiones");

            migrationBuilder.DropColumn(
                name: "TensionPromedio",
                table: "BasesHormigonDimensiones");

            migrationBuilder.DropColumn(
                name: "EsfuerzoCorteX_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "EsfuerzoCorteX_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "EsfuerzoCorteX_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "EsfuerzoCorteY_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "EsfuerzoCorteY_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "EsfuerzoCorteY_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "ModuloBalastoVertical_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "ModuloBalastoVertical_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "ModuloBalastoVertical_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "MomentoX_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "MomentoX_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "MomentoX_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "MomentoY_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "MomentoY_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "MomentoY_Valor",
                table: "BasesHormigon");
        }
    }
}
