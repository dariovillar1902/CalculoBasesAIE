using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CalculoBasesAIE.Models;

public class BaseHormigonReportePDF(
    BaseHormigon baseHormigon,
    BaseHormigonDimensiones dimensiones,
    BaseHormigonCuantia cuantia,
    BaseHormigonArmadura armadura,
    BaseHormigonVerificacionPunzonado punzonado,
    BaseHormigonVerificacionCorte corte,
    bool verificaTension) : IDocument
{
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Size(PageSizes.A4);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header().Text("Informe de Base de Hormigón")
                         .SemiBold().FontSize(18).AlignCenter();

            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(8);

                col.Item().Text("Datos Geométricos").Bold();
                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => c.ConstantColumn(200));

                    AddRow(t, "Área", dimensiones.Area.ToString("0.00"), "m²");
                    AddRow(t, "Ancho X", dimensiones.AnchoX.ToString("0.00"), "m");
                    AddRow(t, "Ancho Y", dimensiones.AnchoY.ToString("0.00"), "m");
                    AddRow(t, "Altura", dimensiones.Altura.ToString("0.00"), "m");
                    AddRow(t, "Verifica Vuelos", dimensiones.VerificaVuelos ? "Sí" : "No");
                });

                col.Item().Text("Verificación de Tensión Admisible").Bold();
                col.Item().Text(verificaTension ? "Cumple" : "No cumple");

                col.Item().Text("Cálculo de Cuantía").Bold();
                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => c.ConstantColumn(250));
                    AddRow(t, "Esfuerzo Axial Mayorado", cuantia.EsfuerzoAxilMayorado.ToString("0.00"), "kN");
                    AddRow(t, "Carga Mayorada", cuantia.CargaMayorada.ToString("0.00"), "kN/m²");
                    AddRow(t, "Momento Mayorado X", cuantia.MomentoMayoradoX.ToString("0.00"), "kN·m");
                    AddRow(t, "Momento Mayorado Y", cuantia.MomentoMayoradoY.ToString("0.00"), "kN·m");
                    AddRow(t, "Área Acero X", cuantia.AreaAceroX.ToString("0.00"), "cm²");
                    AddRow(t, "Área Acero Y", cuantia.AreaAceroY.ToString("0.00"), "cm²");
                });

                col.Item().Text("Verificación de Punzonado").Bold();
                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => c.ConstantColumn(250));
                    AddRow(t, "Carga Total", punzonado.CargaTotal.ToString("0.00"), "kN/m²");
                    AddRow(t, "Resistencia Requerida", punzonado.ResistenciaRequerida.ToString("0.00"), "kN");
                    AddRow(t, "Perímetro Crítico", punzonado.B0.ToString("0.00"), "m");
                    AddRow(t, "Relación Geométrica", punzonado.B.ToString("0.00"));
                    AddRow(t, "Resistencia Nominal", punzonado.ResistenciaNominal.ToString("0.00"), "kN");
                    AddRow(t, "Resistencia de Diseño", punzonado.ResistenciaDiseno.ToString("0.00"), "kN");
                    AddRow(t, "Resultado", punzonado.CumpleVerificacion ? "Cumple" : "No cumple");
                });

                col.Item().Text("Verificación de Corte").Bold();
                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => c.ConstantColumn(250));
                    AddRow(t, "Carga Total", corte.CargaTotal.ToString("0.00"), "kN/m²");
                    AddRow(t, "Resistencia Requerida en X", corte.ResistenciaRequeridaX.ToString("0.00"), "kN");
                    AddRow(t, "Resistencia Requerida en Y", corte.ResistenciaRequeridaY.ToString("0.00"), "kN");
                    AddRow(t, "Resistencia Nominal en X", corte.ResistenciaNominalX.ToString("0.00"), "kN");
                    AddRow(t, "Resistencia Nominal en Y", corte.ResistenciaNominalY.ToString("0.00"), "kN");
                    AddRow(t, "Resistencia de Diseño en X", corte.ResistenciaDisenoX.ToString("0.00"), "kN");
                    AddRow(t, "Resistencia de Diseño en Y", corte.ResistenciaDisenoY.ToString("0.00"), "kN");
                    AddRow(t, "Resultado", corte.CumpleVerificacion ? "Cumple" : "No cumple");
                });

                col.Item().Text("Detalles de Armadura").Bold();
                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => c.ConstantColumn(250));
                    AddRow(t, "Barras en X", armadura.CantidadBarrasX.ToString());
                    AddRow(t, "Barras en Y", armadura.CantidadBarrasY.ToString());
                    AddRow(t, "Separación Barras X", armadura.SeparacionBarrasX.ToString("0.00"), "cm");
                    AddRow(t, "Separación Barras Y", armadura.SeparacionBarrasY.ToString("0.00"), "cm");
                });
            });

            page.Footer().AlignCenter().Text("Documento generado automáticamente · Copilot").FontSize(9);
        });
    }

    private void AddRow(TableDescriptor t, string title, string value, string? unit = null)
    {
        t.Cell().Text(title);
        t.Cell().Text(unit != null ? $"{value} {unit}" : value);
    }
}
