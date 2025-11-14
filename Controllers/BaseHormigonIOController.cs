using Microsoft.AspNetCore.Mvc;
using CalculoBasesAIE.Models;
using CalculoBasesAIE.Services.BaseHormigonService;
using CalculoBasesAIE.Services.BaseHormigonIOService;

namespace CalculoBasesAIE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // Controlador encargado de exportar/importar datos de BasesHormigon
    // en distintos formatos (Excel, CSV, PDF) y de procesar archivos subidos.
#pragma warning disable CS9113 // El parámetro no está leído.
    public class BasesHormigonIOController(BaseHormigonService baseHormigonService, BaseHormigonIOService baseHormigonIOService) : ControllerBase
#pragma warning restore CS9113 // El parámetro no está leído.
    {
        // ============================================================
        // POST: api/BasesHormigonIO/exportExcel/{baseId}
        // Genera y devuelve un archivo Excel con todos los datos,
        // cálculos y verificaciones de una base.
        // ============================================================
        [HttpPost("exportExcel/{baseId}")]
        public async Task<IActionResult> ExportExcel(long baseId)
        {
            var bytes = await baseHormigonIOService.GenerateExcelAsync(baseId);
            if (bytes is null) return NotFound();

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BaseHormigon.xlsx"
            );
        }

        // ============================================================
        // POST: api/BasesHormigonIO/exportCsv/{baseId}
        // Exporta los datos principales de la base en formato CSV.
        // ============================================================
        [HttpPost("exportCsv/{baseId}")]
        public async Task<IActionResult> ExportCsv(long baseId)
        {
            var bytes = await baseHormigonIOService.GenerateCsvAsync(baseId);
            if (bytes == null) return NotFound();

            return File(bytes, "text/csv", "BaseHormigon.csv");
        }

        // ============================================================
        // POST: api/BasesHormigonIO/exportPdf/{baseId}
        // Genera un informe PDF (dimensiones, cálculos, verificaciones,
        // cómputos, etc.).
        // ============================================================
        [HttpPost("exportPdf/{baseId}")]
        public async Task<IActionResult> ExportPdf(long baseId)
        {
            var bytes = await baseHormigonIOService.GeneratePdfAsync(baseId);
            if (bytes is null) return NotFound();

            return File(bytes, "application/pdf", "BaseHormigon.pdf");
        }

        // ============================================================
        // POST: api/BasesHormigonIO/import
        // Permite importar una base desde un archivo subido
        // (CSV, Excel, etc., según la implementación del servicio).
        // Devuelve la base creada, con su ID, usando CreatedAtAction.
        // ============================================================
        [HttpPost("import")]
        public async Task<ActionResult<BaseHormigon>> ImportBaseHormigon(IFormFile file)
        {
            var baseHormigon = await baseHormigonIOService.ImportBaseHormigonAsync(file);
            if (baseHormigon is null)
                return BadRequest("Archivo inválido o formato no soportado");

            return CreatedAtAction(
                actionName: "GetBaseHormigon",
                controllerName: "BasesHormigon",
                routeValues: new { id = baseHormigon.Id },
                value: baseHormigon
            );
        }
    }
}
