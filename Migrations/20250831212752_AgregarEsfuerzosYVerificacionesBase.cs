using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsfuerzosYVerificacionesBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorteX_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorteX_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CorteX_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CorteY_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorteY_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CorteY_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ModuloBalasto_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModuloBalasto_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ModuloBalasto_Valor",
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
                name: "BasesHormigonEsfuerzos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Normal = table.Column<double>(type: "float", nullable: false),
                    MomentoX = table.Column<double>(type: "float", nullable: false),
                    MomentoY = table.Column<double>(type: "float", nullable: false),
                    CorteX = table.Column<double>(type: "float", nullable: false),
                    CorteY = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonEsfuerzos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoeficienteSeguridadVuelco = table.Column<double>(type: "float", nullable: false),
                    VerificaVuelco = table.Column<bool>(type: "bit", nullable: false),
                    CoeficienteSeguridadDeslizamiento = table.Column<double>(type: "float", nullable: false),
                    VerificaDeslizamiento = table.Column<bool>(type: "bit", nullable: false),
                    ExcentricidadX = table.Column<double>(type: "float", nullable: false),
                    ExcentricidadY = table.Column<double>(type: "float", nullable: false),
                    TensionMaximaX = table.Column<double>(type: "float", nullable: false),
                    TensionMinimaX = table.Column<double>(type: "float", nullable: false),
                    TensionMaximaY = table.Column<double>(type: "float", nullable: false),
                    TensionMinimaY = table.Column<double>(type: "float", nullable: false),
                    VerificaTensionAdmisible = table.Column<bool>(type: "bit", nullable: false),
                    AsentamientoMedio = table.Column<double>(type: "float", nullable: false),
                    AsentamientoMaximo = table.Column<double>(type: "float", nullable: false),
                    AsentamientoMinimo = table.Column<double>(type: "float", nullable: false),
                    DistorsionAngular = table.Column<double>(type: "float", nullable: false),
                    VerificaAsentamientoMedio = table.Column<bool>(type: "bit", nullable: false),
                    VerificaAsentamientoDiferencial = table.Column<bool>(type: "bit", nullable: false)
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
                name: "BasesHormigonEsfuerzos");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificaciones");

            migrationBuilder.DropColumn(
                name: "CorteX_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CorteX_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CorteX_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CorteY_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CorteY_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CorteY_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "ModuloBalasto_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "ModuloBalasto_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "ModuloBalasto_Valor",
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
