using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasesHormigon",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EsfuerzoAxil_Valor = table.Column<double>(type: "float", nullable: false),
                    EsfuerzoAxil_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsfuerzoAxil_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeCargaD_Valor = table.Column<double>(type: "float", nullable: false),
                    PorcentajeCargaD_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeCargaD_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeCargaL_Valor = table.Column<double>(type: "float", nullable: false),
                    PorcentajeCargaL_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeCargaL_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnchoColumnaX_Valor = table.Column<double>(type: "float", nullable: false),
                    AnchoColumnaX_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnchoColumnaX_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnchoColumnaY_Valor = table.Column<double>(type: "float", nullable: false),
                    AnchoColumnaY_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnchoColumnaY_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CargaAdmisible_Valor = table.Column<double>(type: "float", nullable: false),
                    CargaAdmisible_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CargaAdmisible_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PesoEspecificoSuelo_Valor = table.Column<double>(type: "float", nullable: false),
                    PesoEspecificoSuelo_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PesoEspecificoSuelo_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NivelFundacion_Valor = table.Column<double>(type: "float", nullable: false),
                    NivelFundacion_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NivelFundacion_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PesoEspecificoHormigon_Valor = table.Column<double>(type: "float", nullable: false),
                    PesoEspecificoHormigon_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PesoEspecificoHormigon_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResistenciaCaracteristicaHormigon_Valor = table.Column<double>(type: "float", nullable: false),
                    ResistenciaCaracteristicaHormigon_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResistenciaCaracteristicaHormigon_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecubrimientoHormigon_Valor = table.Column<double>(type: "float", nullable: false),
                    RecubrimientoHormigon_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecubrimientoHormigon_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TensionFluenciaAcero_Valor = table.Column<double>(type: "float", nullable: false),
                    TensionFluenciaAcero_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TensionFluenciaAcero_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiametroBarrasX_Valor = table.Column<double>(type: "float", nullable: false),
                    DiametroBarrasX_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiametroBarrasX_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiametroBarrasY_Valor = table.Column<double>(type: "float", nullable: false),
                    DiametroBarrasY_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiametroBarrasY_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigon", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasesHormigon");
        }
    }
}
