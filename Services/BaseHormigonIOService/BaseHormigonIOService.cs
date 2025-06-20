using ClosedXML.Excel;
using System.Globalization;
using CalculoBasesAIE.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CalculoBasesAIE.Repositories.BaseHormigonRepository;
using CalculoBasesAIE.Services.BaseHormigonService;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CalculoBasesAIE.Services.BaseHormigonIOService
{
    public class BaseHormigonIOService(IBaseHormigonRepository repository, IBaseHormigonService service) : IBaseHormigonIOService
    {
        public async Task<byte[]?> GenerateExcelAsync(long baseId)
        {
            var baseHormigon = await repository.GetByIdAsync(baseId);
            if (baseHormigon == null) return null;

            var dimensionesBase = service.EstimarDimensiones(baseHormigon);
            var verificaTension = service.VerificarTension(baseHormigon, dimensionesBase);
            var baseHormigonCuantia = service.CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = service.CalcularArmadura(baseHormigon, dimensionesBase, baseHormigonCuantia);
            var verificacionPunzonado = service.VerificarPunzonado(baseHormigon, dimensionesBase);
            var verificacionCorte = service.VerificarCorte(baseHormigon, dimensionesBase);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("BaseHormigon");

            // Add headers
            worksheet.Cell(1, 1).Value = "Area";
            worksheet.Cell(2, 1).Value = "Ancho X";
            worksheet.Cell(3, 1).Value = "Ancho Y";
            worksheet.Cell(4, 1).Value = "Altura";
            worksheet.Cell(5, 1).Value = "Verifica Vuelos";

            worksheet.Cell(1, 2).Value = Math.Round(dimensionesBase.Area, 2);
            worksheet.Cell(2, 2).Value = dimensionesBase.AnchoX;
            worksheet.Cell(3, 2).Value = dimensionesBase.AnchoY;
            worksheet.Cell(4, 2).Value = dimensionesBase.Altura;
            worksheet.Cell(5, 2).Value = dimensionesBase.VerificaVuelos;

            worksheet.Cell(1, 3).Value = "m²";
            worksheet.Cell(2, 3).Value = "m";
            worksheet.Cell(3, 3).Value = "m";
            worksheet.Cell(4, 3).Value = "m";

            worksheet.Cell(7, 1).Value = "Carga Total";
            worksheet.Cell(7, 2).Value = Math.Round(baseHormigon.EsfuerzoAxil.Valor /
                             (dimensionesBase.AnchoX * dimensionesBase.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * dimensionesBase.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - dimensionesBase.Altura), 2);
            worksheet.Cell(8, 1).Value = "Verifica Tensión Admisible";
            worksheet.Cell(8, 2).Value = verificaTension;

            worksheet.Cell(10, 1).Value = "Esfuerzo Axial Mayorado";
            worksheet.Cell(10, 2).Value = Math.Round(baseHormigonCuantia.EsfuerzoAxilMayorado, 2);
            worksheet.Cell(10, 3).Value = "kN";

            worksheet.Cell(11, 1).Value = "Carga Mayorada";
            worksheet.Cell(11, 2).Value = Math.Round(baseHormigonCuantia.CargaMayorada, 2);
            worksheet.Cell(11, 3).Value = "kN/m²";

            worksheet.Cell(12, 1).Value = "Momento Mayorado X";
            worksheet.Cell(12, 2).Value = Math.Round(baseHormigonCuantia.MomentoMayoradoX, 2);
            worksheet.Cell(12, 3).Value = "kN·m";

            worksheet.Cell(13, 1).Value = "Momento Mayorado Y";
            worksheet.Cell(13, 2).Value = Math.Round(baseHormigonCuantia.MomentoMayoradoY, 2);
            worksheet.Cell(13, 3).Value = "kN·m";

            worksheet.Cell(14, 1).Value = "Área Acero X";
            worksheet.Cell(14, 2).Value = Math.Round(baseHormigonCuantia.AreaAceroX, 2);
            worksheet.Cell(14, 3).Value = "cm²";

            worksheet.Cell(15, 1).Value = "Área Acero Y";
            worksheet.Cell(15, 2).Value = Math.Round(baseHormigonCuantia.AreaAceroY, 2);
            worksheet.Cell(15, 3).Value = "cm²";

            worksheet.Cell(17, 1).Value = "Verificación de Punzonado";
            worksheet.Cell(17, 1).Style.Font.Bold = true;

            worksheet.Cell(18, 1).Value = "Esfuerzo Axial Mayorado";
            worksheet.Cell(18, 2).Value = Math.Round(verificacionPunzonado.EsfuerzoAxilMayorado, 2);
            worksheet.Cell(18, 3).Value = "kN";

            worksheet.Cell(19, 1).Value = "Carga Total";
            worksheet.Cell(19, 2).Value = Math.Round(verificacionPunzonado.CargaTotal, 2);
            worksheet.Cell(19, 3).Value = "kN/m²";

            worksheet.Cell(20, 1).Value = "Resistencia Requerida";
            worksheet.Cell(20, 2).Value = Math.Round(verificacionPunzonado.ResistenciaRequerida, 2);
            worksheet.Cell(20, 3).Value = "kN";

            worksheet.Cell(21, 1).Value = "Perímetro Crítico";
            worksheet.Cell(21, 2).Value = Math.Round(verificacionPunzonado.B0, 2);
            worksheet.Cell(21, 3).Value = "m";

            worksheet.Cell(22, 1).Value = "Relación Geométrica";
            worksheet.Cell(22, 2).Value = Math.Round(verificacionPunzonado.B, 2);

            worksheet.Cell(23, 1).Value = "Resistencia Nominal";
            worksheet.Cell(23, 2).Value = Math.Round(verificacionPunzonado.ResistenciaNominal, 2);
            worksheet.Cell(23, 3).Value = "kN";

            worksheet.Cell(24, 1).Value = "Resistencia de Diseño";
            worksheet.Cell(24, 2).Value = Math.Round(verificacionPunzonado.ResistenciaDiseno, 2);
            worksheet.Cell(24, 3).Value = "kN";

            worksheet.Cell(25, 1).Value = "Verifica Punzonado";
            worksheet.Cell(25, 2).Value = verificacionPunzonado.CumpleVerificacion;

            worksheet.Cell(27, 1).Value = "Verificación de Corte";
            worksheet.Cell(27, 1).Style.Font.Bold = true;

            worksheet.Cell(28, 1).Value = "Carga Total";
            worksheet.Cell(28, 2).Value = Math.Round(verificacionCorte.CargaTotal, 2);
            worksheet.Cell(28, 3).Value = "kN/m²";

            worksheet.Cell(29, 1).Value = "Resistencia Requerida en X";
            worksheet.Cell(29, 2).Value = Math.Round(verificacionCorte.ResistenciaRequeridaX, 2);
            worksheet.Cell(29, 3).Value = "kN";

            worksheet.Cell(30, 1).Value = "Resistencia Requerida en Y";
            worksheet.Cell(30, 2).Value = Math.Round(verificacionCorte.ResistenciaRequeridaY, 2);
            worksheet.Cell(30, 3).Value = "kN";

            worksheet.Cell(31, 1).Value = "Resistencia Nominal en X";
            worksheet.Cell(31, 2).Value = Math.Round(verificacionCorte.ResistenciaNominalX, 2);
            worksheet.Cell(31, 3).Value = "kN";

            worksheet.Cell(32, 1).Value = "Resistencia Nominal en Y";
            worksheet.Cell(32, 2).Value = Math.Round(verificacionCorte.ResistenciaNominalY, 2);
            worksheet.Cell(32, 3).Value = "kN";

            worksheet.Cell(33, 1).Value = "Resistencia de Diseño en X";
            worksheet.Cell(33, 2).Value = Math.Round(verificacionCorte.ResistenciaDisenoX, 2);
            worksheet.Cell(33, 3).Value = "kN";

            worksheet.Cell(34, 1).Value = "Resistencia de Diseño en Y";
            worksheet.Cell(34, 2).Value = Math.Round(verificacionCorte.ResistenciaDisenoY, 2);
            worksheet.Cell(34, 3).Value = "kN";

            worksheet.Cell(35, 1).Value = "Verifica Corte";
            worksheet.Cell(35, 2).Value = verificacionCorte.CumpleVerificacion;

            worksheet.Cell(37, 1).Value = "Detalles de Armadura";
            worksheet.Cell(37, 1).Style.Font.Bold = true;

            worksheet.Cell(38, 1).Value = "Barras en X";
            worksheet.Cell(38, 2).Value = calculoArmadura.CantidadBarrasX;

            worksheet.Cell(39, 1).Value = "Barras en Y";
            worksheet.Cell(39, 2).Value = calculoArmadura.CantidadBarrasY;

            worksheet.Cell(40, 1).Value = "Separación Barras X";
            worksheet.Cell(40, 2).Value = Math.Round(calculoArmadura.SeparacionBarrasX, 2);
            worksheet.Cell(40, 3).Value = "cm";

            worksheet.Cell(41, 1).Value = "Separación Barras Y";
            worksheet.Cell(41, 2).Value = Math.Round(calculoArmadura.SeparacionBarrasY, 2);
            worksheet.Cell(41, 3).Value = "cm";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]?> GenerateCsvAsync(long baseId)
        {
            var baseHormigon = await repository.GetByIdAsync(baseId);
            if (baseHormigon == null) return null;

            var dim = service.EstimarDimensiones(baseHormigon);
            var tension = service.VerificarTension(baseHormigon, dim);
            var cuantia = service.CalcularCuantia(baseHormigon, dim);
            var armadura = service.CalcularArmadura(baseHormigon, dim, cuantia);
            var punzonado = service.VerificarPunzonado(baseHormigon, dim);
            var corte = service.VerificarCorte(baseHormigon, dim);

            var sb = new StringBuilder();
            void Add(string label, object? value, string? unit = null)
            {
                value = value switch
                {
                    double d => d.ToString("0.00"),
                    float f => f.ToString("0.00"),
                    decimal m => m.ToString("0.00"),
                    _ => value
                };
                sb.AppendLine($"{label},{value},{unit}");
            }

            Add("Área", Math.Round(dim.Area, 2), "m²");
            Add("Ancho X", dim.AnchoX, "m");
            Add("Ancho Y", dim.AnchoY, "m");
            Add("Altura", dim.Altura, "m");
            Add("Verifica Vuelos", dim.VerificaVuelos);
            sb.AppendLine();

            var cargaTotal = baseHormigon.EsfuerzoAxil.Valor / (dim.AnchoX * dim.AnchoY)
                           + baseHormigon.PesoEspecificoHormigon.Valor * dim.Altura
                           + baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - dim.Altura);
            Add("Carga Total", cargaTotal, "kN/m²");
            Add("Verifica Tensión Admisible", tension);
            sb.AppendLine();

            Add("Esfuerzo Axial Mayorado", cuantia.EsfuerzoAxilMayorado, "kN");
            Add("Carga Mayorada", cuantia.CargaMayorada, "kN/m²");
            Add("Momento Mayorado X", cuantia.MomentoMayoradoX, "kN·m");
            Add("Momento Mayorado Y", cuantia.MomentoMayoradoY, "kN·m");
            Add("Área Acero X", cuantia.AreaAceroX, "cm²");
            Add("Área Acero Y", cuantia.AreaAceroY, "cm²");
            sb.AppendLine();

            Add("Esfuerzo Axial Mayorado", punzonado.EsfuerzoAxilMayorado, "kN");
            Add("Carga Total", punzonado.CargaTotal, "kN/m²");
            Add("Resistencia Requerida", punzonado.ResistenciaRequerida, "kN");
            Add("Perímetro Crítico", punzonado.B0, "m");
            Add("Relación Geométrica", punzonado.B);
            Add("Resistencia Nominal", punzonado.ResistenciaNominal, "kN");
            Add("Resistencia de Diseño", punzonado.ResistenciaDiseno, "kN");
            Add("Verifica Punzonado", punzonado.CumpleVerificacion);
            sb.AppendLine();

            Add("Carga Total", corte.CargaTotal, "kN/m²");
            Add("Resistencia Requerida en X", corte.ResistenciaRequeridaX, "kN");
            Add("Resistencia Requerida en Y", corte.ResistenciaRequeridaY, "kN");
            Add("Resistencia Nominal en X", corte.ResistenciaNominalX, "kN");
            Add("Resistencia Nominal en Y", corte.ResistenciaNominalY, "kN");
            Add("Resistencia de Diseño en X", corte.ResistenciaDisenoX, "kN");
            Add("Resistencia de Diseño en Y", corte.ResistenciaDisenoY, "kN");
            Add("Verifica Corte", corte.CumpleVerificacion);
            sb.AppendLine();

            Add("Barras en X", armadura.CantidadBarrasX);
            Add("Barras en Y", armadura.CantidadBarrasY);
            Add("Separación Barras X", armadura.SeparacionBarrasX, "cm");
            Add("Separación Barras Y", armadura.SeparacionBarrasY, "cm");

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return bytes;
        }

        public async Task<byte[]?> GeneratePdfAsync(long baseId)
        {
            var baseHormigon = await repository.GetByIdAsync(baseId);
            if (baseHormigon == null) return null;

            var dim = service.EstimarDimensiones(baseHormigon);
            var tension = service.VerificarTension(baseHormigon, dim);
            var cuantia = service.CalcularCuantia(baseHormigon, dim);
            var armadura = service.CalcularArmadura(baseHormigon, dim, cuantia);
            var punzonado = service.VerificarPunzonado(baseHormigon, dim);
            var corte = service.VerificarCorte(baseHormigon, dim);

            var pdf = new BaseHormigonReportePDF(
                dim,
                cuantia,
                armadura,
                punzonado,
                corte,
                tension
            );

            using var stream = new MemoryStream();
            QuestPDF.Settings.License = LicenseType.Community;
            pdf.GeneratePdf(stream);
            return stream.ToArray();
        }

        public async Task<BaseHormigon?> ImportBaseHormigonAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            BaseHormigon? baseHormigon = extension switch
            {
                ".csv" => ParseBaseHormigonFromCsv(stream),
                ".xlsx" => ParseBaseHormigonFromExcel(stream),
                _ => null
            };

            if (baseHormigon == null)
                return null;

            service.ConvertirUnidades(baseHormigon);
            var baseHormigonExistente = await repository.GetDuplicateAsync(baseHormigon);

            if (baseHormigonExistente != null)
            {
                return baseHormigonExistente;
            }
            else
            {
                await repository.AddAsync(baseHormigon);
                return baseHormigon;
            }
        }

        public BaseHormigon ParseBaseHormigonFromCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";"
            });

            var records = csv.GetRecords<ValueUnitPairCsv>().ToList();
            var dict = new Dictionary<string, string>();

            foreach (var record in records)
            {
                if (!string.IsNullOrEmpty(record.Variable) && !string.IsNullOrEmpty(record.Valor))
                {
                    dict[record.Variable] = $"{record.Valor}|{record.Unidad}";
                }
            }

            return ParseBaseHormigonFromDictionary(dict);
        }

        public BaseHormigon ParseBaseHormigonFromExcel(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var dict = new Dictionary<string, string>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var key = row.Cell(1).GetString().Trim();
                var value = row.Cell(2).GetString().Trim();
                var unit = row.Cell(3).GetString().Trim();

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    dict[key] = $"{value}|{unit}";
            }

            return ParseBaseHormigonFromDictionary(dict);
        }

        private static BaseHormigon ParseBaseHormigonFromDictionary(Dictionary<string, string> dict)
        {
            ValueUnitPair Parse(string key, string tipo)
            {
                var parts = dict[key].Split('|');
                return new ValueUnitPair
                {
                    Valor = double.Parse(parts[0], CultureInfo.InvariantCulture),
                    Unidad = parts.Length > 1 ? parts[1] : "",
                    Tipo = tipo
                };
            }

            return new BaseHormigon
            {
                EsfuerzoAxil = Parse("Esfuerzo Axil", "fuerza"),
                CargaAdmisible = Parse("Carga Admisible", "presion"),
                PorcentajeCargaD = Parse("Porcentaje Carga D", "porcentaje"),
                PorcentajeCargaL = Parse("Porcentaje Carga L", "porcentaje"),
                AnchoColumnaX = Parse("Ancho Columna X", "longitud"),
                AnchoColumnaY = Parse("Ancho Columna Y", "longitud"),
                PesoEspecificoSuelo = Parse("Peso Especifico Suelo", "densidad"),
                NivelFundacion = Parse("Nivel Fundacion", "longitud"),
                PesoEspecificoHormigon = Parse("Peso Especifico Hormigon", "densidad"),
                ResistenciaCaracteristicaHormigon = Parse("Resistencia Caracteristica Hormigon", "presion"),
                RecubrimientoHormigon = Parse("Recubrimiento Hormigon", "longitud"),
                TensionFluenciaAcero = Parse("Tension Fluencia Acero", "presion"),
                DiametroBarrasX = Parse("Diametro Barras X", "longitud"),
                DiametroBarrasY = Parse("Diametro Barras Y", "longitud")
            };
        }
    }
}