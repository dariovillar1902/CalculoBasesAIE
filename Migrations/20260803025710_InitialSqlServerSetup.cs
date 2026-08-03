using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CalculoBasesAIE");

            migrationBuilder.CreateTable(
                name: "BasesHormigon",
                schema: "CalculoBasesAIE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    DiametroBarrasY_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorteX_Valor = table.Column<double>(type: "float", nullable: false),
                    CorteX_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorteX_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorteY_Valor = table.Column<double>(type: "float", nullable: false),
                    CorteY_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorteY_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MomentoX_Valor = table.Column<double>(type: "float", nullable: false),
                    MomentoX_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MomentoX_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MomentoY_Valor = table.Column<double>(type: "float", nullable: false),
                    MomentoY_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MomentoY_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuloBalasto_Valor = table.Column<double>(type: "float", nullable: false),
                    ModuloBalasto_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuloBalasto_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoM3Hormigon_Valor = table.Column<double>(type: "float", nullable: false),
                    CostoM3Hormigon_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoM3Hormigon_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoKgAcero_Valor = table.Column<double>(type: "float", nullable: false),
                    CostoKgAcero_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoKgAcero_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoM3Excavacion_Valor = table.Column<double>(type: "float", nullable: false),
                    CostoM3Excavacion_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoM3Excavacion_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoeficienteEsponjamiento_Valor = table.Column<double>(type: "float", nullable: false),
                    CoeficienteEsponjamiento_Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoeficienteEsponjamiento_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonArmaduras",
                schema: "CalculoBasesAIE",
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
                name: "BasesHormigonComputos",
                schema: "CalculoBasesAIE",
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

            migrationBuilder.CreateTable(
                name: "BasesHormigonCuantias",
                schema: "CalculoBasesAIE",
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
                schema: "CalculoBasesAIE",
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
                schema: "CalculoBasesAIE",
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
                name: "BasesHormigonEsfuerzos",
                schema: "CalculoBasesAIE",
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
                name: "BasesHormigonVerificacionCorte",
                schema: "CalculoBasesAIE",
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
                name: "BasesHormigonVerificaciones",
                schema: "CalculoBasesAIE",
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

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificacionPunzonado",
                schema: "CalculoBasesAIE",
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

            migrationBuilder.CreateTable(
                name: "TestEntities",
                schema: "CalculoBasesAIE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasesHormigon",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonArmaduras",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonComputos",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonCuantias",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonDiametrosBarras",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonDimensiones",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonEsfuerzos",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificacionCorte",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificaciones",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificacionPunzonado",
                schema: "CalculoBasesAIE");

            migrationBuilder.DropTable(
                name: "TestEntities",
                schema: "CalculoBasesAIE");
        }
    }
}
