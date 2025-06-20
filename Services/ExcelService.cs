using ClosedXML.Excel;
using System.Globalization;
using CalculoBasesAIE.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace CalculoBasesAIE.Services
{
    public class ExcelService : IExcelService
    {
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

        private class ValueUnitPairCsv
        {
            public string Variable { get; set; } = string.Empty;
            public string Valor { get; set; } = string.Empty;
            public string Unidad { get; set; } = string.Empty;
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

        private BaseHormigon ParseBaseHormigonFromDictionary(Dictionary<string, string> dict)
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