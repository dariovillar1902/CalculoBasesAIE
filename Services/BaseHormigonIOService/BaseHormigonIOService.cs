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
            var verificacionesTension = service.VerificarTension(baseHormigon, dimensionesBase);
            var baseHormigonCuantia = service.CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = service.CalcularArmadura(baseHormigon, dimensionesBase, baseHormigonCuantia);
            var verificacionPunzonado = service.VerificarPunzonado(baseHormigon, dimensionesBase);
            var verificacionCorte = service.VerificarCorte(baseHormigon, dimensionesBase);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("BaseHormigon");

            // Dimensiones
            worksheet.Cell(1, 1).Value = "Area";
            worksheet.Cell(1, 2).Value = Math.Round(dimensionesBase.Area, 2);
            worksheet.Cell(1, 3).Value = "m²";

            worksheet.Cell(2, 1).Value = "Ancho X";
            worksheet.Cell(2, 2).Value = dimensionesBase.AnchoX;
            worksheet.Cell(2, 3).Value = "m";

            worksheet.Cell(3, 1).Value = "Ancho Y";
            worksheet.Cell(3, 2).Value = dimensionesBase.AnchoY;
            worksheet.Cell(3, 3).Value = "m";

            worksheet.Cell(4, 1).Value = "Altura";
            worksheet.Cell(4, 2).Value = dimensionesBase.Altura;
            worksheet.Cell(4, 3).Value = "m";

            worksheet.Cell(5, 1).Value = "Vuelo X";
            worksheet.Cell(5, 2).Value = dimensionesBase.VueloX;
            worksheet.Cell(5, 3).Value = "m";

            worksheet.Cell(6, 1).Value = "Vuelo Y";
            worksheet.Cell(6, 2).Value = dimensionesBase.VueloY;
            worksheet.Cell(6, 3).Value = "m";

            worksheet.Cell(7, 1).Value = "Verifica Vuelos";
            worksheet.Cell(7, 2).Value = dimensionesBase.VerificaVuelos;

            // Tensiones
            worksheet.Cell(9, 1).Value = "Tensión X";
            worksheet.Cell(9, 2).Value = Math.Round(verificacionesTension.TensionX, 2);
            worksheet.Cell(9, 3).Value = "kPa";

            worksheet.Cell(10, 1).Value = "Tensión Y";
            worksheet.Cell(10, 2).Value = Math.Round(verificacionesTension.TensionY, 2);
            worksheet.Cell(10, 3).Value = "kPa";

            worksheet.Cell(11, 1).Value = "Verifica Tensión";
            worksheet.Cell(11, 2).Value = verificacionesTension.VerificaTension;
            worksheet.Cell(11, 3).Value = "";

            // Cuantía
            worksheet.Cell(13, 1).Value = "Esfuerzo Axial Mayorado";
            worksheet.Cell(13, 2).Value = Math.Round(baseHormigonCuantia.EsfuerzoAxilMayorado, 2);
            worksheet.Cell(13, 3).Value = "kN";

            worksheet.Cell(14, 1).Value = "Carga Mayorada";
            worksheet.Cell(14, 2).Value = Math.Round(baseHormigonCuantia.CargaMayorada, 2);
            worksheet.Cell(14, 3).Value = "kN/m²";

            worksheet.Cell(15, 1).Value = "Momento Mayorado X";
            worksheet.Cell(15, 2).Value = Math.Round(baseHormigonCuantia.MomentoMayoradoX, 2);
            worksheet.Cell(15, 3).Value = "kN·m";

            worksheet.Cell(16, 1).Value = "Momento Mayorado Y";
            worksheet.Cell(16, 2).Value = Math.Round(baseHormigonCuantia.MomentoMayoradoY, 2);
            worksheet.Cell(16, 3).Value = "kN·m";

            worksheet.Cell(17, 1).Value = "Área Acero X";
            worksheet.Cell(17, 2).Value = Math.Round(baseHormigonCuantia.AreaAceroX, 2);
            worksheet.Cell(17, 3).Value = "cm²";

            worksheet.Cell(18, 1).Value = "Área Acero Y";
            worksheet.Cell(18, 2).Value = Math.Round(baseHormigonCuantia.AreaAceroY, 2);
            worksheet.Cell(18, 3).Value = "cm²";

            // Punzonado y corte (igual que antes)
            // ...

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
            Add("Vuelo X", dim.VueloX, "m");
            Add("Vuelo Y", dim.VueloY, "m");
            Add("Verifica Vuelos", dim.VerificaVuelos);
            sb.AppendLine();

            Add("Tensión X", tension.TensionX, "kPa");
            Add("Tensión Y", tension.TensionY, "kPa");
            Add("Verifica Tensión", tension.VerificaTension);
            sb.AppendLine();

            Add("Esfuerzo Axial Mayorado", cuantia.EsfuerzoAxilMayorado, "kN");
            Add("Carga Mayorada", cuantia.CargaMayorada, "kN/m²");
            Add("Momento Mayorado X", cuantia.MomentoMayoradoX, "kN·m");
            Add("Momento Mayorado Y", cuantia.MomentoMayoradoY, "kN·m");
            Add("Área Acero X", cuantia.AreaAceroX, "cm²");
            Add("Área Acero Y", cuantia.AreaAceroY, "cm²");
            sb.AppendLine();

            Add("Barras en X", armadura.CantidadBarrasX);
            Add("Barras en Y", armadura.CantidadBarrasY);
            Add("Separación Barras X", armadura.SeparacionBarrasX, "cm");
            Add("Separación Barras Y", armadura.SeparacionBarrasY, "cm");
            sb.AppendLine();

            // Punzonado y corte se agregan igual que antes
            // ...

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return bytes;
        }

        public async Task<byte[]?> GeneratePdfAsync(long baseId)
        {
            var baseHormigon = await repository.GetByIdAsync(baseId);
            if (baseHormigon == null) return null;

            var dim = service.EstimarDimensiones(baseHormigon);
            var verificacionesTension = service.VerificarTension(baseHormigon, dim);
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
                verificacionesTension
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

            if (baseHormigon == null) return null;

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
