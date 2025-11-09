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

            var dim = service.EstimarDimensiones(baseHormigon);
            var esfuerzos = service.ObtenerEsfuerzos(baseHormigon, dim);
            var verificaciones = service.VerificarBase(baseHormigon, dim, esfuerzos);
            var cuantia = service.CalcularCuantia(baseHormigon, dim);
            var armadura = service.CalcularArmadura(baseHormigon, dim, cuantia);
            var punzonado = service.VerificarPunzonado(baseHormigon, dim);
            var corte = service.VerificarCorte(baseHormigon, dim);
            var computo = service.Computo(baseHormigon, dim, armadura);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("BaseHormigon");

            worksheet.Cell(1, 1).Value = "Nombre Fundación:";
            worksheet.Cell(1, 2).Value = baseHormigon.Nombre ?? "";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            worksheet.Cell(2, 1).Value = "Fecha Exportación:";
            worksheet.Cell(2, 2).Value = DateTime.Today.ToString("dd/MM/yyyy");
            worksheet.Cell(2, 1).Style.Font.SetBold();

            worksheet.Cell(3, 1).Value = "Empresa / Profesional:";
            worksheet.Cell(3, 2).Value = ""; 
            worksheet.Cell(3, 1).Style.Font.SetBold();

            var startRow = 5;

            // Datos geométricos
            worksheet.Cell(startRow, 1).Value = "Dimensiones de la Base";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Área";
            worksheet.Cell(startRow + 0, 2).Value = Math.Round(dim.Area, 2);
            worksheet.Cell(startRow + 0, 3).Value = "m²";

            worksheet.Cell(startRow + 1, 1).Value = "Ancho X";
            worksheet.Cell(startRow + 1, 2).Value = dim.AnchoX;
            worksheet.Cell(startRow + 1, 3).Value = "m";

            worksheet.Cell(startRow + 2, 1).Value = "Ancho Y";
            worksheet.Cell(startRow + 2, 2).Value = dim.AnchoY;
            worksheet.Cell(startRow + 2, 3).Value = "m";

            worksheet.Cell(startRow + 3, 1).Value = "Altura";
            worksheet.Cell(startRow + 3, 2).Value = dim.Altura;
            worksheet.Cell(startRow + 3, 3).Value = "m";

            startRow += 5;

            // -- Esfuerzos
            worksheet.Cell(startRow, 1).Value = "Esfuerzos en la Base";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Normal";
            worksheet.Cell(startRow + 0, 2).Value = esfuerzos.Normal;
            worksheet.Cell(startRow + 0, 3).Value = "kN";

            worksheet.Cell(startRow + 1, 1).Value = "Momento X";
            worksheet.Cell(startRow + 1, 2).Value = esfuerzos.MomentoX;
            worksheet.Cell(startRow + 1, 3).Value = "kN·m";

            worksheet.Cell(startRow + 2, 1).Value = "Momento Y";
            worksheet.Cell(startRow + 2, 2).Value = esfuerzos.MomentoY;
            worksheet.Cell(startRow + 2, 3).Value = "kN·m";

            worksheet.Cell(startRow + 3, 1).Value = "Corte X";
            worksheet.Cell(startRow + 3, 2).Value = esfuerzos.CorteX;
            worksheet.Cell(startRow + 3, 3).Value = "kN";

            worksheet.Cell(startRow + 4, 1).Value = "Corte Y";
            worksheet.Cell(startRow + 4, 2).Value = esfuerzos.CorteY;
            worksheet.Cell(startRow + 4, 3).Value = "kN";

            startRow += 6;

            // Cálculo de cuantía
            worksheet.Cell(startRow, 1).Value = "Cálculo de Cuantía";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Esfuerzo Axial Mayorado";
            worksheet.Cell(startRow + 0, 2).Value = Math.Round(cuantia.EsfuerzoAxilMayorado, 2);
            worksheet.Cell(startRow + 0, 3).Value = "kN";

            worksheet.Cell(startRow + 1, 1).Value = "Momento Mayorado X";
            worksheet.Cell(startRow + 1, 2).Value = Math.Round(cuantia.MomentoMayoradoX, 2);
            worksheet.Cell(startRow + 1, 3).Value = "kN·m";

            worksheet.Cell(startRow + 2, 1).Value = "Momento Mayorado Y";
            worksheet.Cell(startRow + 2, 2).Value = Math.Round(cuantia.MomentoMayoradoY, 2);
            worksheet.Cell(startRow + 2, 3).Value = "kN·m";

            worksheet.Cell(startRow + 3, 1).Value = "Área Acero X";
            worksheet.Cell(startRow + 3, 2).Value = Math.Round(cuantia.AreaAceroX, 2);
            worksheet.Cell(startRow + 3, 3).Value = "cm²";

            worksheet.Cell(startRow + 4, 1).Value = "Área Acero Y";
            worksheet.Cell(startRow + 4, 2).Value = Math.Round(cuantia.AreaAceroY, 2);
            worksheet.Cell(startRow + 4, 3).Value = "cm²";

            startRow += 6;

            // Verificaciónes (general)
            worksheet.Cell(startRow, 1).Value = "Verificaciones generales";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Coef. Seguridad Vuelco";
            worksheet.Cell(startRow + 0, 2).Value = verificaciones.CoeficienteSeguridadVuelco;

            worksheet.Cell(startRow + 1, 1).Value = "Verifica Vuelco";
            worksheet.Cell(startRow + 1, 2).Value = verificaciones.VerificaVuelco ? "Sí" : "No";

            worksheet.Cell(startRow + 4, 1).Value = "Excentricidad X";
            worksheet.Cell(startRow + 4, 2).Value = verificaciones.ExcentricidadX;
            worksheet.Cell(startRow + 4, 3).Value = "m";

            worksheet.Cell(startRow + 5, 1).Value = "Excentricidad Y";
            worksheet.Cell(startRow + 5, 2).Value = verificaciones.ExcentricidadY;
            worksheet.Cell(startRow + 5, 3).Value = "m";

            worksheet.Cell(startRow + 6, 1).Value = "Tensión Máx X";
            worksheet.Cell(startRow + 6, 2).Value = verificaciones.TensionMaximaX;
            worksheet.Cell(startRow + 6, 3).Value = "kN/m²";

            worksheet.Cell(startRow + 7, 1).Value = "Tensión Mín X";
            worksheet.Cell(startRow + 7, 2).Value = verificaciones.TensionMinimaX;
            worksheet.Cell(startRow + 7, 3).Value = "kN/m²";

            worksheet.Cell(startRow + 8, 1).Value = "Tensión Máx Y";
            worksheet.Cell(startRow + 8, 2).Value = verificaciones.TensionMaximaY;
            worksheet.Cell(startRow + 8, 3).Value = "kN/m²";

            worksheet.Cell(startRow + 9, 1).Value = "Tensión Mín Y";
            worksheet.Cell(startRow + 9, 2).Value = verificaciones.TensionMinimaY;
            worksheet.Cell(startRow + 9, 3).Value = "kN/m²";

            worksheet.Cell(startRow + 10, 1).Value = "Verifica Tensión Admisible";
            worksheet.Cell(startRow + 10, 2).Value = verificaciones.VerificaTensionAdmisible ? "Sí" : "No";

            worksheet.Cell(startRow + 11, 1).Value = "Asentamiento Medio";
            worksheet.Cell(startRow + 11, 2).Value = verificaciones.AsentamientoMedio * 1000;
            worksheet.Cell(startRow + 11, 3).Value = "mm";

            worksheet.Cell(startRow + 12, 1).Value = "Asentamiento Máximo";
            worksheet.Cell(startRow + 12, 2).Value = verificaciones.AsentamientoMaximo * 1000;
            worksheet.Cell(startRow + 12, 3).Value = "mm";

            worksheet.Cell(startRow + 13, 1).Value = "Asentamiento Mínimo";
            worksheet.Cell(startRow + 13, 2).Value = verificaciones.AsentamientoMinimo * 1000;
            worksheet.Cell(startRow + 13, 3).Value = "mm";

            worksheet.Cell(startRow + 14, 1).Value = "Distorsión Angular";
            worksheet.Cell(startRow + 14, 2).Value = verificaciones.DistorsionAngular;

            worksheet.Cell(startRow + 15, 1).Value = "Verifica Asentamiento Medio";
            worksheet.Cell(startRow + 15, 2).Value = verificaciones.VerificaAsentamientoMedio ? "Sí" : "No";

            worksheet.Cell(startRow + 16, 1).Value = "Verifica Asentamiento Diferencial";
            worksheet.Cell(startRow + 16, 2).Value = verificaciones.VerificaAsentamientoDiferencial ? "Sí" : "No";

            startRow += 18;

            // Verificación de Punzonado
            worksheet.Cell(startRow, 1).Value = "Verificación de Punzonado";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Esfuerzo Axial Mayorado";
            worksheet.Cell(startRow + 0, 2).Value = Math.Round(punzonado.EsfuerzoAxilMayorado, 2);
            worksheet.Cell(startRow + 0, 3).Value = "kN";

            worksheet.Cell(startRow + 1, 1).Value = "Carga Total";
            worksheet.Cell(startRow + 1, 2).Value = Math.Round(punzonado.CargaTotal, 2);
            worksheet.Cell(startRow + 1, 3).Value = "kN/m²";

            worksheet.Cell(startRow + 2, 1).Value = "Resistencia Requerida";
            worksheet.Cell(startRow + 2, 2).Value = Math.Round(punzonado.ResistenciaRequerida, 2);
            worksheet.Cell(startRow + 2, 3).Value = "kN";

            worksheet.Cell(startRow + 3, 1).Value = "Perímetro Crítico";
            worksheet.Cell(startRow + 3, 2).Value = Math.Round(punzonado.B0, 2);
            worksheet.Cell(startRow + 3, 3).Value = "m";

            worksheet.Cell(startRow + 4, 1).Value = "Relación Geométrica";
            worksheet.Cell(startRow + 4, 2).Value = Math.Round(punzonado.B, 2);

            worksheet.Cell(startRow + 5, 1).Value = "Resistencia Nominal";
            worksheet.Cell(startRow + 5, 2).Value = Math.Round(punzonado.ResistenciaNominal, 2);
            worksheet.Cell(startRow + 5, 3).Value = "kN";

            worksheet.Cell(startRow + 6, 1).Value = "Resistencia de Diseño";
            worksheet.Cell(startRow + 6, 2).Value = Math.Round(punzonado.ResistenciaDiseno, 2);
            worksheet.Cell(startRow + 6, 3).Value = "kN";

            worksheet.Cell(startRow + 7, 1).Value = "Resultado";
            worksheet.Cell(startRow + 7, 2).Value = punzonado.CumpleVerificacion ? "Cumple" : "No cumple";

            startRow += 9;

            // Verificación de Corte
            worksheet.Cell(startRow, 1).Value = "Verificación de Corte";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Carga Total";
            worksheet.Cell(startRow + 0, 2).Value = Math.Round(corte.CargaTotal, 2);
            worksheet.Cell(startRow + 0, 3).Value = "kN/m²";

            worksheet.Cell(startRow + 1, 1).Value = "Resistencia Requerida en X";
            worksheet.Cell(startRow + 1, 2).Value = Math.Round(corte.ResistenciaRequeridaX, 2);
            worksheet.Cell(startRow + 1, 3).Value = "kN";

            worksheet.Cell(startRow + 2, 1).Value = "Resistencia Requerida en Y";
            worksheet.Cell(startRow + 2, 2).Value = Math.Round(corte.ResistenciaRequeridaY, 2);
            worksheet.Cell(startRow + 2, 3).Value = "kN";

            worksheet.Cell(startRow + 3, 1).Value = "Resistencia Nominal en X";
            worksheet.Cell(startRow + 3, 2).Value = Math.Round(corte.ResistenciaNominalX, 2);
            worksheet.Cell(startRow + 3, 3).Value = "kN";

            worksheet.Cell(startRow + 4, 1).Value = "Resistencia Nominal en Y";
            worksheet.Cell(startRow + 4, 2).Value = Math.Round(corte.ResistenciaNominalY, 2);
            worksheet.Cell(startRow + 4, 3).Value = "kN";

            worksheet.Cell(startRow + 5, 1).Value = "Resistencia de Diseño en X";
            worksheet.Cell(startRow + 5, 2).Value = Math.Round(corte.ResistenciaDisenoX, 2);
            worksheet.Cell(startRow + 5, 3).Value = "kN";

            worksheet.Cell(startRow + 6, 1).Value = "Resistencia de Diseño en Y";
            worksheet.Cell(startRow + 6, 2).Value = Math.Round(corte.ResistenciaDisenoY, 2);
            worksheet.Cell(startRow + 6, 3).Value = "kN";

            worksheet.Cell(startRow + 7, 1).Value = "Resultado";
            worksheet.Cell(startRow + 7, 2).Value = corte.CumpleVerificacion ? "Cumple" : "No cumple";

            startRow += 9;

            // Detalles de armadura
            worksheet.Cell(startRow, 1).Value = "Detalles de Armadura";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Barras en X";
            worksheet.Cell(startRow + 0, 2).Value = armadura.CantidadBarrasX;

            worksheet.Cell(startRow + 1, 1).Value = "Barras en Y";
            worksheet.Cell(startRow + 1, 2).Value = armadura.CantidadBarrasY;

            worksheet.Cell(startRow + 2, 1).Value = "Separación Barras X";
            worksheet.Cell(startRow + 2, 2).Value = Math.Round(armadura.SeparacionBarrasX, 2);
            worksheet.Cell(startRow + 2, 3).Value = "cm";

            worksheet.Cell(startRow + 3, 1).Value = "Separación Barras Y";
            worksheet.Cell(startRow + 3, 2).Value = Math.Round(armadura.SeparacionBarrasY, 2);
            worksheet.Cell(startRow + 3, 3).Value = "cm";

            startRow += 6;

            // Cómputo
            worksheet.Cell(startRow, 1).Value = "Cómputo";
            worksheet.Cell(startRow, 1).Style.Font.SetBold();
            startRow++;

            worksheet.Cell(startRow + 0, 1).Value = "Volumen Hormigón";
            worksheet.Cell(startRow + 0, 2).Value = computo.VolumenHormigon;
            worksheet.Cell(startRow + 0, 3).Value = "m³";

            worksheet.Cell(startRow + 1, 1).Value = "Longitud Barras X";
            worksheet.Cell(startRow + 1, 2).Value = computo.LongitudBarrasX;
            worksheet.Cell(startRow + 1, 3).Value = "m";

            worksheet.Cell(startRow + 2, 1).Value = "Longitud Barras Y";
            worksheet.Cell(startRow + 2, 2).Value = computo.LongitudBarrasY;
            worksheet.Cell(startRow + 2, 3).Value = "m";

            worksheet.Cell(startRow + 3, 1).Value = "Peso Barras X";
            worksheet.Cell(startRow + 3, 2).Value = computo.PesoBarrasX;
            worksheet.Cell(startRow + 3, 3).Value = "kg";

            worksheet.Cell(startRow + 4, 1).Value = "Peso Barras Y";
            worksheet.Cell(startRow + 4, 2).Value = computo.PesoBarrasY;
            worksheet.Cell(startRow + 4, 3).Value = "kg";

            worksheet.Cell(startRow + 5, 1).Value = "Volumen Excavación";
            worksheet.Cell(startRow + 5, 2).Value = computo.VolumenExcavacion;
            worksheet.Cell(startRow + 5, 3).Value = "m³";

            worksheet.Cell(startRow + 6, 1).Value = "Monto Hormigón";
            worksheet.Cell(startRow + 6, 2).Value = computo.MontoHormigon;
            worksheet.Cell(startRow + 6, 3).Value = "$";

            worksheet.Cell(startRow + 7, 1).Value = "Monto Acero";
            worksheet.Cell(startRow + 7, 2).Value = computo.MontoAcero;
            worksheet.Cell(startRow + 7, 3).Value = "$";

            worksheet.Cell(startRow + 8, 1).Value = "Monto Excavación";
            worksheet.Cell(startRow + 8, 2).Value = computo.MontoExcavacion;
            worksheet.Cell(startRow + 8, 3).Value = "$";

            startRow += 9;

            var lastRow = startRow + 11;
            worksheet.Cell(lastRow, 1).Value = "Realizado con app Cálculo Bases AIE · https://link-pendiente";
            worksheet.Range(lastRow, 1, lastRow, 4).Merge().Style.Font.Italic = true;

            // Some basic formatting
            worksheet.Columns().AdjustToContents();
            worksheet.RangeUsed()?.SetAutoFilter();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]?> GenerateCsvAsync(long baseId)
        {
            var baseHormigon = await repository.GetByIdAsync(baseId);
            if (baseHormigon == null) return null;

            var dim = service.EstimarDimensiones(baseHormigon);
            var esfuerzos = service.ObtenerEsfuerzos(baseHormigon, dim);
            var verificaciones = service.VerificarBase(baseHormigon, dim, esfuerzos);
            var cuantia = service.CalcularCuantia(baseHormigon, dim);
            var armadura = service.CalcularArmadura(baseHormigon, dim, cuantia);
            var punzonado = service.VerificarPunzonado(baseHormigon, dim);
            var corte = service.VerificarCorte(baseHormigon, dim);
            var computo = service.Computo(baseHormigon, dim, armadura);

            var sb = new StringBuilder();

            // Header / rótulo
            sb.AppendLine($"Nombre Fundación,{baseHormigon.Nombre}");
            sb.AppendLine($"Fecha Exportación,{DateTime.Today:dd/MM/yyyy}");
            sb.AppendLine("Empresa / Profesional,");
            sb.AppendLine();

            void Add(string label, object? value, string? unit = null)
            {
                value = value switch
                {
                    double d => d.ToString("0.00"),
                    float f => f.ToString("0.00"),
                    decimal m => m.ToString("0.00"),
                    _ => value
                };
                sb.AppendLine($"{EscapeCsv(label)},{EscapeCsv(value?.ToString() ?? "")},{EscapeCsv(unit ?? "")}");
            }

            sb.AppendLine("Dimensiones de la Base");
            // Datos geométricos
            Add("Área", Math.Round(dim.Area, 2), "m²");
            Add("Ancho X", dim.AnchoX, "m");
            Add("Ancho Y", dim.AnchoY, "m");
            Add("Altura", dim.Altura, "m");
            sb.AppendLine();

            // Esfuerzos
            sb.AppendLine("Esfuerzos en la Base");
            Add("Normal", esfuerzos.Normal, "kN");
            Add("Momento X", esfuerzos.MomentoX, "kN·m");
            Add("Momento Y", esfuerzos.MomentoY, "kN·m");
            Add("Corte X", esfuerzos.CorteX, "kN");
            Add("Corte Y", esfuerzos.CorteY, "kN");
            sb.AppendLine();

            // Verificaciones (general)
            sb.AppendLine("Verificaciones Generales");
            Add("Coef. Seguridad Vuelco", verificaciones.CoeficienteSeguridadVuelco);
            Add("Verifica Vuelco", verificaciones.VerificaVuelco ? "Sí" : "No");
            Add("Excentricidad X", verificaciones.ExcentricidadX, "m");
            Add("Excentricidad Y", verificaciones.ExcentricidadY, "m");
            Add("Tensión Máx X", verificaciones.TensionMaximaX, "kN/m²");
            Add("Tensión Mín X", verificaciones.TensionMinimaX, "kN/m²");
            Add("Tensión Máx Y", verificaciones.TensionMaximaY, "kN/m²");
            Add("Tensión Mín Y", verificaciones.TensionMinimaY, "kN/m²");
            Add("Verifica Tensión Admisible", verificaciones.VerificaTensionAdmisible ? "Sí" : "No");
            Add("Asentamiento Medio", verificaciones.AsentamientoMedio * 1000, "mm");
            Add("Asentamiento Máximo", verificaciones.AsentamientoMaximo * 1000, "mm");
            Add("Asentamiento Mínimo", verificaciones.AsentamientoMinimo * 1000, "mm");
            Add("Distorsión Angular", verificaciones.DistorsionAngular);
            Add("Verifica Asentamiento Medio", verificaciones.VerificaAsentamientoMedio ? "Sí" : "No");
            Add("Verifica Asentamiento Diferencial", verificaciones.VerificaAsentamientoDiferencial ? "Sí" : "No");
            sb.AppendLine();

            // Cuantía
            sb.AppendLine("Cálculo de Cuantía");
            Add("Esfuerzo Axial Mayorado", cuantia.EsfuerzoAxilMayorado, "kN");
            Add("Momento Mayorado X", cuantia.MomentoMayoradoX, "kN·m");
            Add("Momento Mayorado Y", cuantia.MomentoMayoradoY, "kN·m");
            Add("Área Acero X", cuantia.AreaAceroX, "cm²");
            Add("Área Acero Y", cuantia.AreaAceroY, "cm²");
            sb.AppendLine();

            // Punzonado
            sb.AppendLine("Verificación de Punzonado");
            Add("Esfuerzo Axial Mayorado", punzonado.EsfuerzoAxilMayorado, "kN");
            Add("Carga Total", punzonado.CargaTotal, "kN/m²");
            Add("Resistencia Requerida", punzonado.ResistenciaRequerida, "kN");
            Add("Perímetro Crítico", punzonado.B0, "m");
            Add("Relación Geométrica", punzonado.B);
            Add("Resistencia Nominal", punzonado.ResistenciaNominal, "kN");
            Add("Resistencia de Diseño", punzonado.ResistenciaDiseno, "kN");
            Add("Verifica Punzonado", punzonado.CumpleVerificacion ? "Sí" : "No");
            sb.AppendLine();

            // Corte
            sb.AppendLine("Verificación de Corte");
            Add("Carga Total", corte.CargaTotal, "kN/m²");
            Add("Resistencia Requerida en X", corte.ResistenciaRequeridaX, "kN");
            Add("Resistencia Requerida en Y", corte.ResistenciaRequeridaY, "kN");
            Add("Resistencia Nominal en X", corte.ResistenciaNominalX, "kN");
            Add("Resistencia Nominal en Y", corte.ResistenciaNominalY, "kN");
            Add("Resistencia de Diseño en X", corte.ResistenciaDisenoX, "kN");
            Add("Resistencia de Diseño en Y", corte.ResistenciaDisenoY, "kN");
            Add("Verifica Corte", corte.CumpleVerificacion ? "Sí" : "No");
            sb.AppendLine();

            // Armadura
            sb.AppendLine("Detalles de Armadura");
            Add("Barras en X", armadura.CantidadBarrasX);
            Add("Barras en Y", armadura.CantidadBarrasY);
            Add("Separación Barras X", armadura.SeparacionBarrasX, "cm");
            Add("Separación Barras Y", armadura.SeparacionBarrasY, "cm");
            sb.AppendLine();

            // Cómputo
            sb.AppendLine("Cómputo");
            Add("Volumen Hormigón", computo.VolumenHormigon, "m³");
            Add("Longitud Barras X", computo.LongitudBarrasX, "m");
            Add("Longitud Barras Y", computo.LongitudBarrasY, "m");
            Add("Peso Barras X", computo.PesoBarrasX, "kg");
            Add("Peso Barras Y", computo.PesoBarrasY, "kg");
            Add("Volumen Excavación", computo.VolumenExcavacion, "m³");
            Add("Monto Hormigón", computo.MontoHormigon, "$");
            Add("Monto Acero", computo.MontoAcero, "$");
            Add("Monto Excavación", computo.MontoExcavacion, "$");
            sb.AppendLine();

            // footer
            sb.AppendLine($"Realizado con app Cálculo Bases AIE,https://link-pendiente");
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return bytes;
        }

        public async Task<byte[]?> GeneratePdfAsync(long baseId)
        {
            var baseHormigon = await repository.GetByIdAsync(baseId);
            if (baseHormigon == null) return null;

            var dim = service.EstimarDimensiones(baseHormigon);
            var esfuerzos = service.ObtenerEsfuerzos(baseHormigon, dim);
            var verificaciones = service.VerificarBase(baseHormigon, dim, esfuerzos);
            var cuantia = service.CalcularCuantia(baseHormigon, dim);
            var armadura = service.CalcularArmadura(baseHormigon, dim, cuantia);
            var punzonado = service.VerificarPunzonado(baseHormigon, dim);
            var corte = service.VerificarCorte(baseHormigon, dim);
            var computo = service.Computo(baseHormigon, dim, armadura);

            var pdf = new BaseHormigonReportePDF(
                baseHormigon.Nombre,
                null,
                dim,
                esfuerzos,
                verificaciones,
                cuantia,
                armadura,
                punzonado,
                corte,
                computo
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
            double ParseDouble(string value)
                => double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0;

            ValueUnitPair? TryParse(string key, string tipo)
            {
                if (!dict.TryGetValue(key, out var raw)) return null;

                var parts = raw.Split('|');
                if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0])) return null;

                return new ValueUnitPair
                {
                    Valor = ParseDouble(parts[0]),
                    Unidad = parts.Length > 1 ? parts[1] : "",
                    Tipo = tipo
                };
            }

            string TryParseString(string key)
                => dict.TryGetValue(key, out var value) ? value.Split('|')[0] : "";

            return new BaseHormigon
            {
                // Basic section
                Nombre = TryParseString("Nombre"),
                EsfuerzoAxil = TryParse("Esfuerzo Axial", "fuerza"),
                CargaAdmisible = TryParse("Carga Admisible", "presion"),
                CorteX = TryParse("Corte X", "fuerza"),
                CorteY = TryParse("Corte Y", "fuerza"),
                MomentoX = TryParse("Momento X", "momento"),
                MomentoY = TryParse("Momento Y", "momento"),
                ModuloBalasto = TryParse("Modulo de Balasto", "presion"),

                // Secondary (may not exist depending on template version)
                PorcentajeCargaD = TryParse("Porcentaje Carga Muerta", "porcentaje"),
                PorcentajeCargaL = TryParse("Porcentaje Carga Viva", "porcentaje"),
                AnchoColumnaX = TryParse("Ancho Columna X", "longitud"),
                AnchoColumnaY = TryParse("Ancho Columna Y", "longitud"),
                ResistenciaCaracteristicaHormigon = TryParse("Resistencia Caracteristica Hormigon", "presion"),
                PesoEspecificoHormigon = TryParse("Peso Especifico Hormigon", "densidad"),
                PesoEspecificoSuelo = TryParse("Peso Especifico Suelo", "densidad"),
                NivelFundacion = TryParse("Nivel Fundacion", "longitud"),
                RecubrimientoHormigon = TryParse("Recubrimiento Hormigon", "longitud"),
                TensionFluenciaAcero = TryParse("Tension Fluencia Acero", "presion"),
                DiametroBarrasX = TryParse("Diametro Barras X", "longitud"),
                DiametroBarrasY = TryParse("Diametro Barras Y", "longitud"),

                // Cost & computation section (optional)
                CostoM3Hormigon = TryParse("Costo m³ Hormigón", "costo"),
                CostoKgAcero = TryParse("Costo kg Acero", "costo"),
                CostoM3Excavacion = TryParse("Costo m³ Excavación", "costo"),
                CoeficienteEsponjamiento = TryParse("Coeficiente Esponjamiento", "coeficiente")
            };
        }

        private static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            var needsQuotes = input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r');
            if (!needsQuotes) return input;
            var escaped = input.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }
    }
}