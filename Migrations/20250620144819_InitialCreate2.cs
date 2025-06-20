using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasesHormigonArmaduras",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CantidadBarrasX = table.Column<double>(type: "float", nullable: false),
                    CantidadBarrasY = table.Column<double>(type: "float", nullable: false),
                    SeparacionBarrasX = table.Column<double>(type: "float", nullable: false),
                    SeparacionBarrasY = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonArmaduras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonCuantias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EsfuerzoAxilMayorado = table.Column<double>(type: "float", nullable: false),
                    CargaMayorada = table.Column<double>(type: "float", nullable: false),
                    MomentoMayoradoX = table.Column<double>(type: "float", nullable: false),
                    MomentoMayoradoY = table.Column<double>(type: "float", nullable: false),
                    MomentoNominalX = table.Column<double>(type: "float", nullable: false),
                    MomentoNominalY = table.Column<double>(type: "float", nullable: false),
                    FactorAdimensionalX = table.Column<double>(type: "float", nullable: false),
                    FactorAdimensionalY = table.Column<double>(type: "float", nullable: false),
                    CuantiaMecanicaX = table.Column<double>(type: "float", nullable: false),
                    CuantiaMecanicaY = table.Column<double>(type: "float", nullable: false),
                    CuantiaCalculoX = table.Column<double>(type: "float", nullable: false),
                    CuantiaCalculoY = table.Column<double>(type: "float", nullable: false),
                    CuantiaMaxima = table.Column<double>(type: "float", nullable: false),
                    VerificaCuantiaMaxima = table.Column<bool>(type: "bit", nullable: false),
                    CuantiaMinima = table.Column<double>(type: "float", nullable: false),
                    CuantiaAdoptadaX = table.Column<double>(type: "float", nullable: false),
                    CuantiaAdoptadaY = table.Column<double>(type: "float", nullable: false),
                    AreaAceroX = table.Column<double>(type: "float", nullable: false),
                    AreaAceroY = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonCuantias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonDiametrosBarras",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiametroX = table.Column<double>(type: "float", nullable: false),
                    DiametroY = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonDiametrosBarras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonDimensiones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<double>(type: "float", nullable: false),
                    AnchoX = table.Column<double>(type: "float", nullable: false),
                    AnchoY = table.Column<double>(type: "float", nullable: false),
                    VueloX = table.Column<double>(type: "float", nullable: false),
                    VueloY = table.Column<double>(type: "float", nullable: false),
                    VerificaVuelos = table.Column<bool>(type: "bit", nullable: false),
                    Altura = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonDimensiones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificacionCorte",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargaTotal = table.Column<double>(type: "float", nullable: false),
                    ResistenciaRequeridaX = table.Column<double>(type: "float", nullable: false),
                    ResistenciaRequeridaY = table.Column<double>(type: "float", nullable: false),
                    ResistenciaNominalX = table.Column<double>(type: "float", nullable: false),
                    ResistenciaNominalY = table.Column<double>(type: "float", nullable: false),
                    ResistenciaDisenoX = table.Column<double>(type: "float", nullable: false),
                    ResistenciaDisenoY = table.Column<double>(type: "float", nullable: false),
                    CumpleVerificacion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonVerificacionCorte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificacionPunzonado",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EsfuerzoAxilMayorado = table.Column<double>(type: "float", nullable: false),
                    CargaTotal = table.Column<double>(type: "float", nullable: false),
                    ResistenciaRequerida = table.Column<double>(type: "float", nullable: false),
                    B0 = table.Column<double>(type: "float", nullable: false),
                    B = table.Column<double>(type: "float", nullable: false),
                    ResistenciaNominal = table.Column<double>(type: "float", nullable: false),
                    ResistenciaDiseno = table.Column<double>(type: "float", nullable: false),
                    CumpleVerificacion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonVerificacionPunzonado", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasesHormigonArmaduras");

            migrationBuilder.DropTable(
                name: "BasesHormigonCuantias");

            migrationBuilder.DropTable(
                name: "BasesHormigonDiametrosBarras");

            migrationBuilder.DropTable(
                name: "BasesHormigonDimensiones");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificacionCorte");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificacionPunzonado");
        }
    }
}
