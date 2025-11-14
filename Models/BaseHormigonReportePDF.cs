using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace CalculoBasesAIE.Models
{
    public class BaseHormigonReportePDF(
        string nombreFundacion,
        string? empresaUsuario,
        BaseHormigonDimensiones dimensiones,
        BaseHormigonEsfuerzos esfuerzos,
        BaseHormigonVerificaciones verificaciones,
        BaseHormigonCuantia cuantia,
        BaseHormigonArmadura armadura,
        BaseHormigonVerificacionPunzonado punzonado,
        BaseHormigonVerificacionCorte corte,
        BaseHormigonComputo computo
    ) : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Configuración general de la página
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                // ---------- HEADER ----------
                page.Header().Border(1).Padding(8).Column(col =>
                {
                    col.Spacing(4);

                    // Fila superior: título y empresa
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Cálculo Base de Hormigón").FontSize(18).SemiBold();
                        row.ConstantItem(120).Border(1).Height(50).AlignCenter()
                           .AlignMiddle().Text(empresaUsuario ?? "Profesional / Empresa");
                    });

                    // Fila inferior: nombre fundación y fecha
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Nombre: {nombreFundacion}");
                        row.ConstantItem(120).Text($"Fecha: {DateTime.Today:dd/MM/yyyy}");
                    });
                });

                // ---------- CONTENIDO ----------
                page.Content().PaddingVertical(12).Column(col =>
                {
                    col.Spacing(14);

                    // Cada sección se renderiza llamando a Section con su propio Column
                    Section(col, "Datos Geométricos", t =>
                    {
                        AddRow(t, "Área", dimensiones.Area.ToString("0.00"), "m²");
                        AddRow(t, "Ancho X", dimensiones.AnchoX.ToString("0.00"), "m");
                        AddRow(t, "Ancho Y", dimensiones.AnchoY.ToString("0.00"), "m");
                        AddRow(t, "Altura", dimensiones.Altura.ToString("0.00"), "m");
                    });

                    Section(col, "Esfuerzos Actuantes", t =>
                    {
                        AddRow(t, "Normal N", esfuerzos.Normal.ToString("0.00"), "kN");
                        AddRow(t, "Momento X", esfuerzos.MomentoX.ToString("0.00"), "kN·m");
                        AddRow(t, "Momento Y", esfuerzos.MomentoY.ToString("0.00"), "kN·m");
                        AddRow(t, "Corte X", esfuerzos.CorteX.ToString("0.00"), "kN");
                        AddRow(t, "Corte Y", esfuerzos.CorteY.ToString("0.00"), "kN");
                    });

                    Section(col, "Verificaciones Globales", t =>
                    {
                        AddRow(t, "Coef. Seguridad Vuelco", verificaciones.CoeficienteSeguridadVuelco.ToString("0.00"));
                        AddRow(t, "Verifica Vuelco", verificaciones.VerificaVuelco ? "Sí ✓" : "No ✗");
                        AddRow(t, "Excentricidad X", verificaciones.ExcentricidadX.ToString("0.00"), "m");
                        AddRow(t, "Excentricidad Y", verificaciones.ExcentricidadY.ToString("0.00"), "m");
                        AddRow(t, "Tensión Máx X", verificaciones.TensionMaximaX.ToString("0.00"), "kg/cm²");
                        AddRow(t, "Tensión Mín X", verificaciones.TensionMinimaX.ToString("0.00"), "kg/cm²");
                        AddRow(t, "Tensión Máx Y", verificaciones.TensionMaximaY.ToString("0.00"), "kg/cm²");
                        AddRow(t, "Tensión Mín Y", verificaciones.TensionMinimaY.ToString("0.00"), "kg/cm²");
                        AddRow(t, "Verifica Tensión Admisible", verificaciones.VerificaTensionAdmisible ? "Sí ✓" : "No ✗");
                        AddRow(t, "Asentamiento Medio", verificaciones.AsentamientoMedio.ToString("0.00"), "cm");
                        AddRow(t, "Asentamiento Máximo", verificaciones.AsentamientoMaximo.ToString("0.00"), "cm");
                        AddRow(t, "Asentamiento Mínimo", verificaciones.AsentamientoMinimo.ToString("0.00"), "cm");
                        AddRow(t, "Distorsión Angular", verificaciones.DistorsionAngular.ToString("0.000"));
                        AddRow(t, "Verifica Asentamiento Medio", verificaciones.VerificaAsentamientoMedio ? "Sí ✓" : "No ✗");
                        AddRow(t, "Verifica Asentamiento Diferencial", verificaciones.VerificaAsentamientoDiferencial ? "Sí ✓" : "No ✗");
                    });

                    Section(col, "Cálculo de Cuantía", t =>
                    {
                        AddRow(t, "Esfuerzo Axial Mayorado", cuantia.EsfuerzoAxilMayorado.ToString("0.00"), "kN");
                        AddRow(t, "Momento Mayorado X", cuantia.MomentoMayoradoX.ToString("0.00"), "kN·m");
                        AddRow(t, "Momento Mayorado Y", cuantia.MomentoMayoradoY.ToString("0.00"), "kN·m");
                        AddRow(t, "Área Acero X", cuantia.AreaAceroX.ToString("0.00"), "cm²");
                        AddRow(t, "Área Acero Y", cuantia.AreaAceroY.ToString("0.00"), "cm²");
                    });

                    Section(col, "Verificación de Punzonado", t =>
                    {
                        AddRow(t, "Resistencia Requerida", punzonado.ResistenciaRequerida.ToString("0.00"), "kN");
                        AddRow(t, "Resistencia de Diseño", punzonado.ResistenciaDiseno.ToString("0.00"), "kN");
                        AddRow(t, "Resultado", punzonado.CumpleVerificacion ? "Cumple ✓" : "No cumple ✗");
                    });

                    Section(col, "Verificación de Corte", t =>
                    {
                        AddRow(t, "Resistencia Requerida X", corte.ResistenciaRequeridaX.ToString("0.00"), "kN");
                        AddRow(t, "Resistencia Requerida Y", corte.ResistenciaRequeridaY.ToString("0.00"), "kN");
                        AddRow(t, "Resistencia Diseño X", corte.ResistenciaDisenoX.ToString("0.00"), "kN");
                        AddRow(t, "Resistencia Diseño Y", corte.ResistenciaDisenoY.ToString("0.00"), "kN");
                        AddRow(t, "Resultado", corte.CumpleVerificacion ? "Cumple ✓" : "No cumple ✗");
                    });

                    Section(col, "Detalles de Armadura", t =>
                    {
                        AddRow(t, "Barras en X", armadura.CantidadBarrasX.ToString());
                        AddRow(t, "Barras en Y", armadura.CantidadBarrasY.ToString());
                        AddRow(t, "Separación X", armadura.SeparacionBarrasX.ToString("0.00"), "cm");
                        AddRow(t, "Separación Y", armadura.SeparacionBarrasY.ToString("0.00"), "cm");
                    });

                    Section(col, "Cómputo de Materiales", t =>
                    {
                        AddRow(t, "Volumen de Hormigón", computo.VolumenHormigon.ToString("0.00"), "m³");
                        AddRow(t, "Peso Acero X", computo.PesoBarrasX.ToString("0.00"), "kg");
                        AddRow(t, "Peso Acero Y", computo.PesoBarrasY.ToString("0.00"), "kg");
                        AddRow(t, "Volumen Excavación", computo.VolumenExcavacion.ToString("0.00"), "m³");
                        AddRow(t, "Costo Hormigón", computo.MontoHormigon.ToString("0.00"), "$");
                        AddRow(t, "Costo Acero", computo.MontoAcero.ToString("0.00"), "$");
                        AddRow(t, "Costo Excavación", computo.MontoExcavacion.ToString("0.00"), "$");
                    });
                });

                // ---------- FOOTER ----------
                page.Footer().AlignCenter()
                    .Text("Realizado con app Cálculo Bases AIE · https://calculo-bases-aie.vercel.app/")
                    .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
            });
        }

        // ---------- MÉTODO PARA SECCIONES ----------
        private static void Section(ColumnDescriptor col, string title, Action<TableDescriptor> table)
        {
            // Título de la sección
            col.Item().PaddingBottom(4).Text(title).SemiBold().FontSize(13).Underline();

            // Tabla de dos columnas
            col.Item().Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(1);   // Columna 1
                    c.RelativeColumn(1.2f); // Columna 2
                });

                // Llamamos al delegate que agrega filas
                table(t);
            });
        }

        // ---------- MÉTODO PARA FILAS DE TABLA ----------
        private static void AddRow(TableDescriptor t, string title, string value, string? unit = null)
        {
            t.Cell().Text(title);
            t.Cell().Text(unit != null ? $"{value} {unit}" : value);
        }
    }
}
