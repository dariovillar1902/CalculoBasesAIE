using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalculoBasesAIE.Models;
using ClosedXML.Excel;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using CalculoBasesAIE.Services;
using DocumentFormat.OpenXml.InkML;

namespace CalculoBasesAIE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesHormigonController(BaseHormigonContext context, IExcelService excelService) : ControllerBase
    {

        // GET: api/BasesHormigon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BaseHormigon>>> GetBasesHormigon()
        {
            return await context.BasesHormigon.ToListAsync();
        }

        // GET: api/BasesHormigon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseHormigon>> GetBaseHormigon(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);

            if (baseHormigon == null)
            {
                return NotFound();
            }

            return baseHormigon;
        }

        // Get estimated dimensions
        [HttpGet("{id}/dimensionesBase")]
        public async Task<ActionResult<BaseHormigonDimensiones>> GetDimensionesBase(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            return Ok(dimensionesBase);
        }

        // Get tension verification
        [HttpGet("{id}/verificaTensionAdmisible")]
        public async Task<ActionResult<bool>> GetVerificaTensionAdmisible(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificaTension = VerificarTension(baseHormigon, dimensionesBase);
            return Ok(verificaTension);
        }

        [HttpGet("{id}/calculoCuantia")]
        public async Task<ActionResult<BaseHormigonCuantia>> GetCalculoCuantia(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var baseHormigonCuantia = CalcularCuantia(baseHormigon, dimensionesBase);

            return Ok(baseHormigonCuantia);
        }

        [HttpGet("{id}/calculoArmadura")]
        public async Task<ActionResult<BaseHormigonArmadura>> GetCalculoArmadura(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = CalcularArmadura(baseHormigon, dimensionesBase, cuantia);

            return Ok(calculoArmadura);
        }

        [HttpGet("{id}/verificaPunzonado")]
        public async Task<ActionResult<BaseHormigonVerificacionPunzonado>> GetVerificaPunzonado(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificacionPunzonado = VerificarPunzonado(baseHormigon, dimensionesBase);

            return Ok(verificacionPunzonado);
        }

        [HttpGet("{id}/verificaCorte")]
        public async Task<ActionResult<BaseHormigonVerificacionCorte>> GetVerificaCorte(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificacionCorte = VerificarCorte(baseHormigon, dimensionesBase);

            return Ok(verificacionCorte);
        }

        // PUT: api/BasesHormigon/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBaseHormigon(long id, BaseHormigon baseHormigon)
        {
            if (id != baseHormigon.Id)
            {
                return BadRequest();
            }

            context.Entry(baseHormigon).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BaseHormigonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BaseHormigon>> PostBaseHormigon(BaseHormigon baseHormigon)
        {
            ConvertirUnidades(baseHormigon);
            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificaTensionAdmisible = VerificarTension(baseHormigon, dimensionesBase);
            var calculoCuantia = CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = CalcularArmadura(baseHormigon, dimensionesBase, calculoCuantia);
            var verificaPunzonado = VerificarPunzonado(baseHormigon, dimensionesBase);
            var verificaCorte = VerificarCorte(baseHormigon, dimensionesBase);

            context.BasesHormigon.Add(baseHormigon);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBaseHormigon), new { id = baseHormigon.Id }, baseHormigon);
        }

        // DELETE: api/BasesHormigon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBaseHormigon(long id)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null)
            {
                return NotFound();
            }

            context.BasesHormigon.Remove(baseHormigon);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("exportExcel/{baseId}")]
        public async Task<IActionResult> GenerateExcelAsync(long baseId)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(baseId);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificaTension = VerificarTension(baseHormigon, dimensionesBase);
            var baseHormigonCuantia = CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = CalcularArmadura(baseHormigon, dimensionesBase, baseHormigonCuantia);
            var verificacionPunzonado = VerificarPunzonado(baseHormigon, dimensionesBase);
            var verificacionCorte = VerificarCorte(baseHormigon, dimensionesBase);

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
            stream.Seek(0, SeekOrigin.Begin);

            var content = stream.ToArray();
            return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "BaseHormigon.xlsx");
        }

        [HttpPost("exportCsv/{baseId}")]
        public async Task<IActionResult> GenerateCsvAsync(long baseId)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(baseId);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificaTension = VerificarTension(baseHormigon, dimensionesBase);
            var baseHormigonCuantia = CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = CalcularArmadura(baseHormigon, dimensionesBase, baseHormigonCuantia);
            var verificacionPunzonado = VerificarPunzonado(baseHormigon, dimensionesBase);
            var verificacionCorte = VerificarCorte(baseHormigon, dimensionesBase);

            var csv = new StringBuilder();

            void AppendRow(string label, object? value, string? unit = null)
            {
                if (value is double d)
                    value = string.Format("{0:0.00}", d);
                else if (value is float f)
                    value = string.Format("{0:0.00}", f);
                else if (value is decimal m)
                    value = string.Format("{0:0.00}", m);

                csv.AppendLine($"{label},{value},{unit}");
            }

            // General
            AppendRow("Area", Math.Round(dimensionesBase.Area, 2), "m²");
            AppendRow("Ancho X", dimensionesBase.AnchoX, "m");
            AppendRow("Ancho Y", dimensionesBase.AnchoY, "m");
            AppendRow("Altura", dimensionesBase.Altura, "m");
            AppendRow("Verifica Vuelos", dimensionesBase.VerificaVuelos);

            csv.AppendLine(); // Separator
            AppendRow("Carga Total",
                Math.Round(baseHormigon.EsfuerzoAxil.Valor /
                           (dimensionesBase.AnchoX * dimensionesBase.AnchoY) +
                           baseHormigon.PesoEspecificoHormigon.Valor * dimensionesBase.Altura +
                           baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - dimensionesBase.Altura),
                    2),
                "kN/m²");
            AppendRow("Verifica Tensión Admisible", verificaTension);

            csv.AppendLine();
            AppendRow("Esfuerzo Axial Mayorado", baseHormigonCuantia.EsfuerzoAxilMayorado, "kN");
            AppendRow("Carga Mayorada", baseHormigonCuantia.CargaMayorada, "kN/m²");
            AppendRow("Momento Mayorado X", baseHormigonCuantia.MomentoMayoradoX, "kN·m");
            AppendRow("Momento Mayorado Y", baseHormigonCuantia.MomentoMayoradoY, "kN·m");
            AppendRow("Área Acero X", baseHormigonCuantia.AreaAceroX, "cm²");
            AppendRow("Área Acero Y", baseHormigonCuantia.AreaAceroY, "cm²");

            csv.AppendLine();
            AppendRow("Esfuerzo Axial Mayorado", verificacionPunzonado.EsfuerzoAxilMayorado, "kN");
            AppendRow("Carga Total", verificacionPunzonado.CargaTotal, "kN/m²");
            AppendRow("Resistencia Requerida", verificacionPunzonado.ResistenciaRequerida, "kN");
            AppendRow("Perímetro Crítico", verificacionPunzonado.B0, "m");
            AppendRow("Relación Geométrica", verificacionPunzonado.B);
            AppendRow("Resistencia Nominal", verificacionPunzonado.ResistenciaNominal, "kN");
            AppendRow("Resistencia de Diseño", verificacionPunzonado.ResistenciaDiseno, "kN");
            AppendRow("Verifica Punzonado", verificacionPunzonado.CumpleVerificacion);

            csv.AppendLine();
            AppendRow("Carga Total", verificacionCorte.CargaTotal, "kN/m²");
            AppendRow("Resistencia Requerida en X", verificacionCorte.ResistenciaRequeridaX, "kN");
            AppendRow("Resistencia Requerida en Y", verificacionCorte.ResistenciaRequeridaY, "kN");
            AppendRow("Resistencia Nominal en X", verificacionCorte.ResistenciaNominalX, "kN");
            AppendRow("Resistencia Nominal en Y", verificacionCorte.ResistenciaNominalY, "kN");
            AppendRow("Resistencia de Diseño en X", verificacionCorte.ResistenciaDisenoX, "kN");
            AppendRow("Resistencia de Diseño en Y", verificacionCorte.ResistenciaDisenoY, "kN");
            AppendRow("Verifica Corte", verificacionCorte.CumpleVerificacion);

            csv.AppendLine();
            AppendRow("Barras en X", calculoArmadura.CantidadBarrasX);
            AppendRow("Barras en Y", calculoArmadura.CantidadBarrasY);
            AppendRow("Separación Barras X", calculoArmadura.SeparacionBarrasX, "cm");
            AppendRow("Separación Barras Y", calculoArmadura.SeparacionBarrasY, "cm");

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", "BaseHormigon.csv");
        }

        [HttpPost("exportPdf/{baseId}")]
        public async Task<IActionResult> GeneratePdfAsync(long baseId)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(baseId);
            if (baseHormigon == null) return NotFound();

            var dim = EstimarDimensiones(baseHormigon);
            var tension = VerificarTension(baseHormigon, dim);
            var cuantia = CalcularCuantia(baseHormigon, dim);
            var armadura = CalcularArmadura(baseHormigon, dim, cuantia);
            var punzonado = VerificarPunzonado(baseHormigon, dim);
            var corte = VerificarCorte(baseHormigon, dim);

            var document = new BaseHormigonReportePDF(baseHormigon, dim, cuantia, armadura, punzonado, corte, tension);
            var stream = new MemoryStream();
            QuestPDF.Settings.License = LicenseType.Community;
            document.GeneratePdf(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "application/pdf", "BaseHormigon.pdf");
        }

        [HttpPost("import")]
        public async Task<ActionResult<BaseHormigon>> ImportBaseHormigon(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            BaseHormigon baseHormigon;

            if (extension == ".csv")
            {
                baseHormigon = excelService.ParseBaseHormigonFromCsv(stream);
            }
            else if (extension == ".xlsx")
            {
                baseHormigon = excelService.ParseBaseHormigonFromExcel(stream);
            }
            else
            {
                return BadRequest("Formato no soportado");
            }

            ConvertirUnidades(baseHormigon);
            context.BasesHormigon.Add(baseHormigon);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBaseHormigon), new { id = baseHormigon.Id }, baseHormigon);
        }

        [HttpPost("calculoArmadura/{id}")]
        public async Task<ActionResult<BaseHormigonArmadura>> CalcularArmaduraCustom(
            long id,
            [FromBody] BaseHormigonDiametrosBarras nuevosDiametros)
        {
            var baseHormigon = await context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            baseHormigon.DiametroBarrasX.Valor = nuevosDiametros.DiametroX / 1000;
            baseHormigon.DiametroBarrasY.Valor = nuevosDiametros.DiametroY / 1000;

            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);
            var armadura = CalcularArmadura(baseHormigon, dim, cuantia);

            return Ok(armadura);
        }



        private bool BaseHormigonExists(long id)
        {
            return context.BasesHormigon.Any(e => e.Id == id);
        }

        private static void ConvertirUnidades(BaseHormigon baseHormigon)
        {
            // Se convierte la unidad a kN
            if (baseHormigon.EsfuerzoAxil.Unidad == "N")
            {
                baseHormigon.EsfuerzoAxil.Valor = baseHormigon.EsfuerzoAxil.Valor / 1000;
                baseHormigon.EsfuerzoAxil.Unidad = "kN";
            }

            // Se convierte la unidad a kPa
            if (baseHormigon.CargaAdmisible.Unidad == "MPa")
            {
                baseHormigon.CargaAdmisible.Valor = baseHormigon.CargaAdmisible.Valor * 1000;
                baseHormigon.CargaAdmisible.Unidad = "kPa";
            } else if (baseHormigon.CargaAdmisible.Unidad == "Pa")
            {
                baseHormigon.CargaAdmisible.Valor = baseHormigon.CargaAdmisible.Valor / 1000;
                baseHormigon.CargaAdmisible.Unidad = "kPa";
            }

            // Se convierte la unidad a %
            if (baseHormigon.PorcentajeCargaD.Unidad == "-")
            {
                baseHormigon.PorcentajeCargaD.Valor = baseHormigon.PorcentajeCargaD.Valor * 100;
                baseHormigon.PorcentajeCargaD.Unidad = "%";
            }

            if (baseHormigon.PorcentajeCargaL.Unidad == "-")
            {
                baseHormigon.PorcentajeCargaL.Valor = baseHormigon.PorcentajeCargaL.Valor * 100;
                baseHormigon.PorcentajeCargaL.Unidad = "%";
            }

            // Se convierte la unidad a m
            if (baseHormigon.AnchoColumnaX.Unidad == "cm")
            {
                baseHormigon.AnchoColumnaX.Valor = baseHormigon.AnchoColumnaX.Valor / 100;
                baseHormigon.AnchoColumnaX.Unidad = "m";
            }

            if (baseHormigon.AnchoColumnaY.Unidad == "cm")
            {
                baseHormigon.AnchoColumnaY.Valor = baseHormigon.AnchoColumnaY.Valor / 100;
                baseHormigon.AnchoColumnaY.Unidad = "m";
            }

            if (baseHormigon.DiametroBarrasX.Unidad == "mm")
            {
                baseHormigon.DiametroBarrasX.Valor = baseHormigon.DiametroBarrasX.Valor / 1000;
                baseHormigon.DiametroBarrasX.Unidad = "m";
            }

            if (baseHormigon.DiametroBarrasY.Unidad == "mm")
            {
                baseHormigon.DiametroBarrasY.Valor = baseHormigon.DiametroBarrasY.Valor / 1000;
                baseHormigon.DiametroBarrasY.Unidad = "m";
            }

            if (baseHormigon.DiametroBarrasX.Unidad == "cm")
            {
                baseHormigon.DiametroBarrasX.Valor = baseHormigon.DiametroBarrasX.Valor / 100;
                baseHormigon.DiametroBarrasX.Unidad = "m";
            }

            if (baseHormigon.DiametroBarrasY.Unidad == "cm")
            {
                baseHormigon.DiametroBarrasY.Valor = baseHormigon.DiametroBarrasY.Valor / 100;
                baseHormigon.DiametroBarrasY.Unidad = "m";
            }

            if (baseHormigon.PesoEspecificoSuelo.Unidad == "N/m3")
            {
                baseHormigon.PesoEspecificoSuelo.Valor = baseHormigon.PesoEspecificoSuelo.Valor / 1000;
                baseHormigon.PesoEspecificoSuelo.Unidad = "kN/m3";
            }

            if (baseHormigon.NivelFundacion.Unidad == "cm")
            {
                baseHormigon.NivelFundacion.Valor = baseHormigon.NivelFundacion.Valor / 100;
                baseHormigon.NivelFundacion.Unidad = "m";
            }

            if (baseHormigon.PesoEspecificoHormigon.Unidad == "N/m3")
            {
                baseHormigon.PesoEspecificoHormigon.Valor = baseHormigon.PesoEspecificoHormigon.Valor / 1000;
                baseHormigon.PesoEspecificoHormigon.Unidad = "kN/m3";
            }

            if (baseHormigon.ResistenciaCaracteristicaHormigon.Unidad == "MPa")
            {
                baseHormigon.ResistenciaCaracteristicaHormigon.Valor = baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000;
                baseHormigon.ResistenciaCaracteristicaHormigon.Unidad = "kPa";
            }
            else if (baseHormigon.ResistenciaCaracteristicaHormigon.Unidad == "Pa")
            {
                baseHormigon.ResistenciaCaracteristicaHormigon.Valor = baseHormigon.ResistenciaCaracteristicaHormigon.Valor / 1000;
                baseHormigon.ResistenciaCaracteristicaHormigon.Unidad = "kPa";
            }

            if (baseHormigon.RecubrimientoHormigon.Unidad == "cm")
            {
                baseHormigon.RecubrimientoHormigon.Valor = baseHormigon.RecubrimientoHormigon.Valor / 100;
                baseHormigon.RecubrimientoHormigon.Unidad = "m";
            }

            if (baseHormigon.TensionFluenciaAcero.Unidad == "MPa")
            {
                baseHormigon.TensionFluenciaAcero.Valor = baseHormigon.TensionFluenciaAcero.Valor * 1000;
                baseHormigon.TensionFluenciaAcero.Unidad = "kPa";
            }
            else if (baseHormigon.TensionFluenciaAcero.Unidad == "Pa")
            {
                baseHormigon.TensionFluenciaAcero.Valor = baseHormigon.TensionFluenciaAcero.Valor / 1000;
                baseHormigon.TensionFluenciaAcero.Unidad = "kPa";
            }
        }

        private static BaseHormigonDimensiones EstimarDimensiones(BaseHormigon baseHormigon)
        {
            var baseHormigonDimensiones = new BaseHormigonDimensiones
            {
                Area = 1.05 * baseHormigon.EsfuerzoAxil.Valor /
                       (baseHormigon.CargaAdmisible.Valor - baseHormigon.PesoEspecificoSuelo.Valor * baseHormigon.NivelFundacion.Valor)
            };

            baseHormigonDimensiones.AnchoX = Math.Sqrt(baseHormigonDimensiones.Area +
                    (Math.Pow(baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor, 2)) / 4)
                    + (baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor) / 2;

            baseHormigonDimensiones.AnchoY = Math.Sqrt(baseHormigonDimensiones.Area +
                    (Math.Pow(baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor, 2)) / 4)
                    - (baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor) / 2;

            baseHormigonDimensiones.AnchoX = Math.Ceiling(baseHormigonDimensiones.AnchoX * 10) / 10;
            baseHormigonDimensiones.AnchoY = Math.Ceiling(baseHormigonDimensiones.AnchoY * 10) / 10;

            baseHormigonDimensiones.VueloX = baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor;
            baseHormigonDimensiones.VueloY = baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor;
            baseHormigonDimensiones.VerificaVuelos = Math.Abs(baseHormigonDimensiones.VueloX - baseHormigonDimensiones.VueloY) < 0.2;

            var condicionesAltura = new double[]
            {
        (baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor) / 5,
        (baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor) / 5,
        0.25
            };

            baseHormigonDimensiones.Altura = Math.Ceiling(condicionesAltura.Max() * 20) / 20;

            return baseHormigonDimensiones;
        }


        private static bool VerificarTension(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var cargaTotal = baseHormigon.EsfuerzoAxil.Valor /
                             (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            return cargaTotal < baseHormigon.CargaAdmisible.Valor;
        }


        private static BaseHormigonCuantia CalcularCuantia(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var baseHormigonCuantia = new BaseHormigonCuantia
            {
                EsfuerzoAxilMayorado = new double[]
                {
            1.4 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor,
            1.2 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor +
            1.6 * (baseHormigon.PorcentajeCargaL.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor
                }.Max()
            };

            baseHormigonCuantia.CargaMayorada = baseHormigonCuantia.EsfuerzoAxilMayorado /
                (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);

            baseHormigonCuantia.MomentoMayoradoX = baseHormigonCuantia.CargaMayorada *
                Math.Pow(baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor, 2) *
                baseHormigonDimensiones.AnchoY / 8;

            baseHormigonCuantia.MomentoMayoradoY = baseHormigonCuantia.CargaMayorada *
                Math.Pow(baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor, 2) *
                baseHormigonDimensiones.AnchoX / 8;

            baseHormigonCuantia.MomentoNominalX = baseHormigonCuantia.MomentoMayoradoX / 0.9;
            baseHormigonCuantia.MomentoNominalY = baseHormigonCuantia.MomentoMayoradoY / 0.9;

            baseHormigonCuantia.FactorAdimensionalX = baseHormigonCuantia.MomentoNominalX /
                (baseHormigonDimensiones.AnchoY * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02), 2) *
                 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000);

            baseHormigonCuantia.FactorAdimensionalY = baseHormigonCuantia.MomentoNominalY /
                (baseHormigonDimensiones.AnchoX * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02), 2) *
                 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000);

            baseHormigonCuantia.CuantiaMecanicaX = 1 - Math.Sqrt(1 - 2 * baseHormigonCuantia.FactorAdimensionalX);
            baseHormigonCuantia.CuantiaMecanicaY = 1 - Math.Sqrt(1 - 2 * baseHormigonCuantia.FactorAdimensionalY);

            baseHormigonCuantia.CuantiaCalculoX = baseHormigonCuantia.CuantiaMecanicaX *
                0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor /
                (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            baseHormigonCuantia.CuantiaCalculoY = baseHormigonCuantia.CuantiaMecanicaY *
                0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor /
                (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            baseHormigonCuantia.CuantiaMaxima = 0.85 * 3 * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor /
                (8 * (baseHormigon.TensionFluenciaAcero.Valor / 1000));

            baseHormigonCuantia.VerificaCuantiaMaxima = baseHormigonCuantia.CuantiaCalculoX < baseHormigonCuantia.CuantiaMaxima &&
                                                        baseHormigonCuantia.CuantiaCalculoY < baseHormigonCuantia.CuantiaMaxima;

            baseHormigonCuantia.CuantiaMinima = 1.4 / (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            baseHormigonCuantia.CuantiaAdoptadaX = Math.Min(Math.Max(baseHormigonCuantia.CuantiaCalculoX, baseHormigonCuantia.CuantiaMinima), 4 / 3.0 * baseHormigonCuantia.CuantiaCalculoX);
            baseHormigonCuantia.CuantiaAdoptadaY = Math.Min(Math.Max(baseHormigonCuantia.CuantiaCalculoY, baseHormigonCuantia.CuantiaMinima), 4 / 3.0 * baseHormigonCuantia.CuantiaCalculoY);

            baseHormigonCuantia.AreaAceroX = baseHormigonCuantia.CuantiaAdoptadaX * baseHormigonDimensiones.AnchoY *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) * 10000;

            baseHormigonCuantia.AreaAceroY = baseHormigonCuantia.CuantiaAdoptadaY * baseHormigonDimensiones.AnchoX *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) * 10000;

            return baseHormigonCuantia;
        }

        private static BaseHormigonArmadura CalcularArmadura(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonCuantia baseHormigonCuantia)
        {
            var diametrosBarras = (baseHormigon.DiametroBarrasX.Valor, baseHormigon.DiametroBarrasY.Valor);

            var areasBarrasIndividuales = (Math.PI * Math.Pow(diametrosBarras.Item1, 2) / 4, Math.PI * Math.Pow(diametrosBarras.Item2, 2) / 4);
            
            var cantidadBarras = (
                Math.Ceiling(baseHormigonCuantia.AreaAceroX / (10000 * areasBarrasIndividuales.Item1)),
                Math.Ceiling(baseHormigonCuantia.AreaAceroY / (10000 * areasBarrasIndividuales.Item2))
            );

            var separacionBarras = (
                Math.Round(100 * Math.Min((baseHormigonDimensiones.AnchoY - 2 * baseHormigon.RecubrimientoHormigon.Valor) / (cantidadBarras.Item1 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item1, 0.3 }.Min())),

                Math.Round(100 * Math.Min((baseHormigonDimensiones.AnchoX - 2 * baseHormigon.RecubrimientoHormigon.Valor) / (cantidadBarras.Item2 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item2, 0.3 }.Min()))
            );

            var baseHormigonArmadura = new BaseHormigonArmadura()
            {
                CantidadBarrasX = cantidadBarras.Item1,
                CantidadBarrasY = cantidadBarras.Item2,
                SeparacionBarrasX = separacionBarras.Item1,
                SeparacionBarrasY = separacionBarras.Item2,
            };

            return baseHormigonArmadura;
        }

        private static BaseHormigonVerificacionPunzonado VerificarPunzonado(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var verificacionPunzonado = new BaseHormigonVerificacionPunzonado();

            var combinacionesCarga = new double[]
            {
        1.4 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor,
        1.2 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor +
        1.6 * (baseHormigon.PorcentajeCargaL.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor
            };

            verificacionPunzonado.EsfuerzoAxilMayorado = combinacionesCarga.Max();

            verificacionPunzonado.CargaTotal = baseHormigon.EsfuerzoAxil.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                                               baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                                               baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            verificacionPunzonado.ResistenciaRequerida = verificacionPunzonado.EsfuerzoAxilMayorado - verificacionPunzonado.CargaTotal *
                (baseHormigon.AnchoColumnaX.Valor + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02)) *
                (baseHormigon.AnchoColumnaY.Valor + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02));

            verificacionPunzonado.B0 = 2 * (baseHormigon.AnchoColumnaX.Valor + baseHormigon.AnchoColumnaY.Valor) +
                                       4 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02);

            verificacionPunzonado.B = new double[] { baseHormigon.AnchoColumnaX.Valor, baseHormigon.AnchoColumnaY.Valor }.Max() /
                                      new double[] { baseHormigon.AnchoColumnaX.Valor, baseHormigon.AnchoColumnaY.Valor }.Min();

            var resistenciasNominales = new double[]
            {
        verificacionPunzonado.B0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
        Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 3,

        (1 + 2 / verificacionPunzonado.B) * verificacionPunzonado.B0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
        Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6,

        (2 + 40 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) / verificacionPunzonado.B0) *
        verificacionPunzonado.B0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
        Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 12
            };

            verificacionPunzonado.ResistenciaNominal = resistenciasNominales.Min();
            verificacionPunzonado.ResistenciaDiseno = verificacionPunzonado.ResistenciaNominal * 0.75;

            verificacionPunzonado.CumpleVerificacion = verificacionPunzonado.ResistenciaRequerida <= verificacionPunzonado.ResistenciaDiseno;

            return verificacionPunzonado;
        }

        private static BaseHormigonVerificacionCorte VerificarCorte(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var verificacionCorte = new BaseHormigonVerificacionCorte
            {
                CargaTotal = baseHormigon.EsfuerzoAxil.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                                           baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                                           baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura)
            };

            verificacionCorte.ResistenciaRequeridaX = verificacionCorte.CargaTotal *
                (((baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor) / 2 -
                 (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02))) * baseHormigonDimensiones.AnchoY;

            verificacionCorte.ResistenciaRequeridaY = verificacionCorte.CargaTotal *
                (((baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor) / 2 -
                 (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02))) * baseHormigonDimensiones.AnchoX;

            verificacionCorte.ResistenciaNominalX = baseHormigonDimensiones.AnchoY *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
                Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6;

            verificacionCorte.ResistenciaNominalY = baseHormigonDimensiones.AnchoX *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
                Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6;

            verificacionCorte.ResistenciaDisenoX = verificacionCorte.ResistenciaNominalX * 0.75;
            verificacionCorte.ResistenciaDisenoY = verificacionCorte.ResistenciaNominalY * 0.75;

            verificacionCorte.CumpleVerificacion = verificacionCorte.ResistenciaRequeridaX <= verificacionCorte.ResistenciaDisenoX &&
                                                   verificacionCorte.ResistenciaRequeridaY <= verificacionCorte.ResistenciaDisenoY;

            return verificacionCorte;
        }
    }
}
