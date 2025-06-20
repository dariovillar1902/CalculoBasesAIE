using Microsoft.AspNetCore.Mvc;
using CalculoBasesAIE.Models;
using CalculoBasesAIE.Services.BaseHormigonService;
using CalculoBasesAIE.Services.BaseHormigonIOService;

namespace CalculoBasesAIE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
#pragma warning disable CS9113 // El parámetro no está leído.
    public class BasesHormigonIOController(BaseHormigonService baseHormigonService, BaseHormigonIOService baseHormigonIOService) : ControllerBase
#pragma warning restore CS9113 // El parámetro no está leído.
    {
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

        [HttpPost("exportCsv/{baseId}")]
        public async Task<IActionResult> ExportCsv(long baseId)
        {
            var bytes = await baseHormigonIOService.GenerateCsvAsync(baseId);
            if (bytes == null) return NotFound();

            return File(bytes, "text/csv", "BaseHormigon.csv");
        }

        [HttpPost("exportPdf/{baseId}")]
        public async Task<IActionResult> ExportPdf(long baseId)
        {
            var bytes = await baseHormigonIOService.GeneratePdfAsync(baseId);
            if (bytes is null) return NotFound();

            return File(bytes, "application/pdf", "BaseHormigon.pdf");
        }

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
