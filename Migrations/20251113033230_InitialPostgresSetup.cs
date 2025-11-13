using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CalculoBasesAIE.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasesHormigon",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    EsfuerzoAxil_Valor = table.Column<double>(type: "double precision", nullable: false),
                    EsfuerzoAxil_Unidad = table.Column<string>(type: "text", nullable: false),
                    EsfuerzoAxil_Tipo = table.Column<string>(type: "text", nullable: false),
                    PorcentajeCargaD_Valor = table.Column<double>(type: "double precision", nullable: false),
                    PorcentajeCargaD_Unidad = table.Column<string>(type: "text", nullable: false),
                    PorcentajeCargaD_Tipo = table.Column<string>(type: "text", nullable: false),
                    PorcentajeCargaL_Valor = table.Column<double>(type: "double precision", nullable: false),
                    PorcentajeCargaL_Unidad = table.Column<string>(type: "text", nullable: false),
                    PorcentajeCargaL_Tipo = table.Column<string>(type: "text", nullable: false),
                    AnchoColumnaX_Valor = table.Column<double>(type: "double precision", nullable: false),
                    AnchoColumnaX_Unidad = table.Column<string>(type: "text", nullable: false),
                    AnchoColumnaX_Tipo = table.Column<string>(type: "text", nullable: false),
                    AnchoColumnaY_Valor = table.Column<double>(type: "double precision", nullable: false),
                    AnchoColumnaY_Unidad = table.Column<string>(type: "text", nullable: false),
                    AnchoColumnaY_Tipo = table.Column<string>(type: "text", nullable: false),
                    CargaAdmisible_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CargaAdmisible_Unidad = table.Column<string>(type: "text", nullable: false),
                    CargaAdmisible_Tipo = table.Column<string>(type: "text", nullable: false),
                    PesoEspecificoSuelo_Valor = table.Column<double>(type: "double precision", nullable: false),
                    PesoEspecificoSuelo_Unidad = table.Column<string>(type: "text", nullable: false),
                    PesoEspecificoSuelo_Tipo = table.Column<string>(type: "text", nullable: false),
                    NivelFundacion_Valor = table.Column<double>(type: "double precision", nullable: false),
                    NivelFundacion_Unidad = table.Column<string>(type: "text", nullable: false),
                    NivelFundacion_Tipo = table.Column<string>(type: "text", nullable: false),
                    PesoEspecificoHormigon_Valor = table.Column<double>(type: "double precision", nullable: false),
                    PesoEspecificoHormigon_Unidad = table.Column<string>(type: "text", nullable: false),
                    PesoEspecificoHormigon_Tipo = table.Column<string>(type: "text", nullable: false),
                    ResistenciaCaracteristicaHormigon_Valor = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaCaracteristicaHormigon_Unidad = table.Column<string>(type: "text", nullable: false),
                    ResistenciaCaracteristicaHormigon_Tipo = table.Column<string>(type: "text", nullable: false),
                    RecubrimientoHormigon_Valor = table.Column<double>(type: "double precision", nullable: false),
                    RecubrimientoHormigon_Unidad = table.Column<string>(type: "text", nullable: false),
                    RecubrimientoHormigon_Tipo = table.Column<string>(type: "text", nullable: false),
                    TensionFluenciaAcero_Valor = table.Column<double>(type: "double precision", nullable: false),
                    TensionFluenciaAcero_Unidad = table.Column<string>(type: "text", nullable: false),
                    TensionFluenciaAcero_Tipo = table.Column<string>(type: "text", nullable: false),
                    DiametroBarrasX_Valor = table.Column<double>(type: "double precision", nullable: false),
                    DiametroBarrasX_Unidad = table.Column<string>(type: "text", nullable: false),
                    DiametroBarrasX_Tipo = table.Column<string>(type: "text", nullable: false),
                    DiametroBarrasY_Valor = table.Column<double>(type: "double precision", nullable: false),
                    DiametroBarrasY_Unidad = table.Column<string>(type: "text", nullable: false),
                    DiametroBarrasY_Tipo = table.Column<string>(type: "text", nullable: false),
                    CorteX_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CorteX_Unidad = table.Column<string>(type: "text", nullable: false),
                    CorteX_Tipo = table.Column<string>(type: "text", nullable: false),
                    CorteY_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CorteY_Unidad = table.Column<string>(type: "text", nullable: false),
                    CorteY_Tipo = table.Column<string>(type: "text", nullable: false),
                    MomentoX_Valor = table.Column<double>(type: "double precision", nullable: false),
                    MomentoX_Unidad = table.Column<string>(type: "text", nullable: false),
                    MomentoX_Tipo = table.Column<string>(type: "text", nullable: false),
                    MomentoY_Valor = table.Column<double>(type: "double precision", nullable: false),
                    MomentoY_Unidad = table.Column<string>(type: "text", nullable: false),
                    MomentoY_Tipo = table.Column<string>(type: "text", nullable: false),
                    ModuloBalasto_Valor = table.Column<double>(type: "double precision", nullable: false),
                    ModuloBalasto_Unidad = table.Column<string>(type: "text", nullable: false),
                    ModuloBalasto_Tipo = table.Column<string>(type: "text", nullable: false),
                    CostoM3Hormigon_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CostoM3Hormigon_Unidad = table.Column<string>(type: "text", nullable: false),
                    CostoM3Hormigon_Tipo = table.Column<string>(type: "text", nullable: false),
                    CostoKgAcero_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CostoKgAcero_Unidad = table.Column<string>(type: "text", nullable: false),
                    CostoKgAcero_Tipo = table.Column<string>(type: "text", nullable: false),
                    CostoM3Excavacion_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CostoM3Excavacion_Unidad = table.Column<string>(type: "text", nullable: false),
                    CostoM3Excavacion_Tipo = table.Column<string>(type: "text", nullable: false),
                    CoeficienteEsponjamiento_Valor = table.Column<double>(type: "double precision", nullable: false),
                    CoeficienteEsponjamiento_Unidad = table.Column<string>(type: "text", nullable: false),
                    CoeficienteEsponjamiento_Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonArmaduras",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CantidadBarrasX = table.Column<double>(type: "double precision", nullable: false),
                    CantidadBarrasY = table.Column<double>(type: "double precision", nullable: false),
                    SeparacionBarrasX = table.Column<double>(type: "double precision", nullable: false),
                    SeparacionBarrasY = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonArmaduras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonComputos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VolumenHormigon = table.Column<double>(type: "double precision", nullable: false),
                    LongitudBarrasX = table.Column<double>(type: "double precision", nullable: false),
                    LongitudBarrasY = table.Column<double>(type: "double precision", nullable: false),
                    PesoBarrasX = table.Column<double>(type: "double precision", nullable: false),
                    PesoBarrasY = table.Column<double>(type: "double precision", nullable: false),
                    VolumenExcavacion = table.Column<double>(type: "double precision", nullable: false),
                    MontoHormigon = table.Column<double>(type: "double precision", nullable: false),
                    MontoAcero = table.Column<double>(type: "double precision", nullable: false),
                    MontoExcavacion = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonComputos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonCuantias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EsfuerzoAxilMayorado = table.Column<double>(type: "double precision", nullable: false),
                    CargaMayorada = table.Column<double>(type: "double precision", nullable: false),
                    MomentoMayoradoX = table.Column<double>(type: "double precision", nullable: false),
                    MomentoMayoradoY = table.Column<double>(type: "double precision", nullable: false),
                    MomentoNominalX = table.Column<double>(type: "double precision", nullable: false),
                    MomentoNominalY = table.Column<double>(type: "double precision", nullable: false),
                    FactorAdimensionalX = table.Column<double>(type: "double precision", nullable: false),
                    FactorAdimensionalY = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaMecanicaX = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaMecanicaY = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaCalculoX = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaCalculoY = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaMaxima = table.Column<double>(type: "double precision", nullable: false),
                    VerificaCuantiaMaxima = table.Column<bool>(type: "boolean", nullable: false),
                    CuantiaMinima = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaAdoptadaX = table.Column<double>(type: "double precision", nullable: false),
                    CuantiaAdoptadaY = table.Column<double>(type: "double precision", nullable: false),
                    AreaAceroX = table.Column<double>(type: "double precision", nullable: false),
                    AreaAceroY = table.Column<double>(type: "double precision", nullable: false)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiametroX = table.Column<double>(type: "double precision", nullable: false),
                    DiametroY = table.Column<double>(type: "double precision", nullable: false)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    AnchoX = table.Column<double>(type: "double precision", nullable: false),
                    AnchoY = table.Column<double>(type: "double precision", nullable: false),
                    VueloX = table.Column<double>(type: "double precision", nullable: false),
                    VueloY = table.Column<double>(type: "double precision", nullable: false),
                    VerificaVuelos = table.Column<bool>(type: "boolean", nullable: false),
                    Altura = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonDimensiones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonEsfuerzos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Normal = table.Column<double>(type: "double precision", nullable: false),
                    MomentoX = table.Column<double>(type: "double precision", nullable: false),
                    MomentoY = table.Column<double>(type: "double precision", nullable: false),
                    CorteX = table.Column<double>(type: "double precision", nullable: false),
                    CorteY = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonEsfuerzos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificacionCorte",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CargaTotal = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaRequeridaX = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaRequeridaY = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaNominalX = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaNominalY = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaDisenoX = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaDisenoY = table.Column<double>(type: "double precision", nullable: false),
                    CumpleVerificacion = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonVerificacionCorte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoeficienteSeguridadVuelco = table.Column<double>(type: "double precision", nullable: false),
                    VerificaVuelco = table.Column<bool>(type: "boolean", nullable: false),
                    CoeficienteSeguridadDeslizamiento = table.Column<double>(type: "double precision", nullable: false),
                    VerificaDeslizamiento = table.Column<bool>(type: "boolean", nullable: false),
                    ExcentricidadX = table.Column<double>(type: "double precision", nullable: false),
                    ExcentricidadY = table.Column<double>(type: "double precision", nullable: false),
                    TensionMaximaX = table.Column<double>(type: "double precision", nullable: false),
                    TensionMinimaX = table.Column<double>(type: "double precision", nullable: false),
                    TensionMaximaY = table.Column<double>(type: "double precision", nullable: false),
                    TensionMinimaY = table.Column<double>(type: "double precision", nullable: false),
                    VerificaTensionAdmisible = table.Column<bool>(type: "boolean", nullable: false),
                    AsentamientoMedio = table.Column<double>(type: "double precision", nullable: false),
                    AsentamientoMaximo = table.Column<double>(type: "double precision", nullable: false),
                    AsentamientoMinimo = table.Column<double>(type: "double precision", nullable: false),
                    DistorsionAngular = table.Column<double>(type: "double precision", nullable: false),
                    VerificaAsentamientoMedio = table.Column<bool>(type: "boolean", nullable: false),
                    VerificaAsentamientoDiferencial = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonVerificaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasesHormigonVerificacionPunzonado",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EsfuerzoAxilMayorado = table.Column<double>(type: "double precision", nullable: false),
                    CargaTotal = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaRequerida = table.Column<double>(type: "double precision", nullable: false),
                    B0 = table.Column<double>(type: "double precision", nullable: false),
                    B = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaNominal = table.Column<double>(type: "double precision", nullable: false),
                    ResistenciaDiseno = table.Column<double>(type: "double precision", nullable: false),
                    CumpleVerificacion = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesHormigonVerificacionPunzonado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
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
                name: "BasesHormigon");

            migrationBuilder.DropTable(
                name: "BasesHormigonArmaduras");

            migrationBuilder.DropTable(
                name: "BasesHormigonComputos");

            migrationBuilder.DropTable(
                name: "BasesHormigonCuantias");

            migrationBuilder.DropTable(
                name: "BasesHormigonDiametrosBarras");

            migrationBuilder.DropTable(
                name: "BasesHormigonDimensiones");

            migrationBuilder.DropTable(
                name: "BasesHormigonEsfuerzos");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificacionCorte");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificaciones");

            migrationBuilder.DropTable(
                name: "BasesHormigonVerificacionPunzonado");

            migrationBuilder.DropTable(
                name: "TestEntities");
        }
    }
}
