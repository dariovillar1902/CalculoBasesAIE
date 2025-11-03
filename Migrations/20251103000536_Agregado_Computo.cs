using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class Agregado_Computo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoeficienteEsponjamiento_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoeficienteEsponjamiento_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CoeficienteEsponjamiento_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CostoKgAcero_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CostoKgAcero_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CostoKgAcero_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CostoM3Excavacion_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CostoM3Excavacion_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CostoM3Excavacion_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CostoM3Hormigon_Tipo",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CostoM3Hormigon_Unidad",
                table: "BasesHormigon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CostoM3Hormigon_Valor",
                table: "BasesHormigon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "BasesHormigonComputos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VolumenHormigon = table.Column<double>(type: "float", nullable: false),
                    LongitudBarrasX = table.Column<double>(type: "float", nullable: false),
                    LongitudBarrasY = table.Column<double>(type: "float", nullable: false),
                    PesoBarrasX = table.Column<double>(type: "float", nullable: false),
                    PesoBarrasY = table.Column<double>(type: "float", nullable: false),
                    VolumenExcavacion = table.Column<double>(type: "float", nullable: false),
                    MontoHormigon = table.Column<double>(type: "float", nullable: false),
                    MontoAcero = table.Column<double>(type: "float", nullable: false),
                    MontoExcavacion = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonComputos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasesHormigonComputos");

            migrationBuilder.DropColumn(
                name: "CoeficienteEsponjamiento_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CoeficienteEsponjamiento_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CoeficienteEsponjamiento_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoKgAcero_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoKgAcero_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoKgAcero_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoM3Excavacion_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoM3Excavacion_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoM3Excavacion_Valor",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoM3Hormigon_Tipo",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoM3Hormigon_Unidad",
                table: "BasesHormigon");

            migrationBuilder.DropColumn(
                name: "CostoM3Hormigon_Valor",
                table: "BasesHormigon");
        }
    }
}
