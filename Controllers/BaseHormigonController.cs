using Microsoft.AspNetCore.Mvc;
using CalculoBasesAIE.Models;
using CalculoBasesAIE.Services.BaseHormigonService;

namespace CalculoBasesAIE.Controllers
{
    // Controlador encargado de manejar todas las operaciones relacionadas
    // con las bases de hormigón: lectura, cálculos, verificaciones y CRUD.
    [Route("api/[controller]")]
    [ApiController]
    public class BasesHormigonController : ControllerBase
    {
        private readonly IBaseHormigonService _baseHormigonService;

        public BasesHormigonController(IBaseHormigonService baseHormigonService)
        {
            _baseHormigonService = baseHormigonService;
        }

        // ================================================
        // GET: api/baseshormigon
        // Devuelve todas las bases registradas
        // ================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BaseHormigon>>> GetBasesHormigon()
        {
            var items = await _baseHormigonService.GetAllBasesAsync();
            return Ok(items);
        }

        // ================================================
        // GET: api/baseshormigon/{id}
        // Obtiene una base por su ID
        // ================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseHormigon>> GetBaseHormigon(long id)
        {
            var item = await _baseHormigonService.GetBaseByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/dimensionesBase
        // Devuelve el cálculo de dimensiones de la base
        // ================================================
        [HttpGet("{id}/dimensionesBase")]
        public async Task<ActionResult<BaseHormigonDimensiones>> GetDimensionesBase(long id)
        {
            var dim = await _baseHormigonService.GetDimensionesAsync(id);
            return dim is null ? NotFound() : Ok(dim);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/esfuerzosBase
        // Devuelve los esfuerzos internos en la base
        // ================================================
        [HttpGet("{id}/esfuerzosBase")]
        public async Task<ActionResult<BaseHormigonEsfuerzos>> GetEsfuerzosBase(long id)
        {
            var result = await _baseHormigonService.GetEsfuerzosAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/verificacionesBase
        // Ejecuta y devuelve verificaciones generales
        // (capacidad portante, tensiones, estabilidad, etc.)
        // ================================================
        [HttpGet("{id}/verificacionesBase")]
        public async Task<ActionResult<BaseHormigonVerificaciones>> GetVerificacionesBase(long id)
        {
            var result = await _baseHormigonService.VerificarBaseAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/calculoCuantia
        // Devuelve el cálculo de cuantía mínima o requerida
        // ================================================
        [HttpGet("{id}/calculoCuantia")]
        public async Task<ActionResult<BaseHormigonCuantia>> GetCalculoCuantia(long id)
        {
            var cuantia = await _baseHormigonService.CalcularCuantiaAsync(id);
            return cuantia is null ? NotFound() : Ok(cuantia);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/calculoArmadura
        // Devuelve la armadura calculada (barras y distribución)
        // ================================================
        [HttpGet("{id}/calculoArmadura")]
        public async Task<ActionResult<BaseHormigonArmadura>> GetCalculoArmadura(long id)
        {
            var armadura = await _baseHormigonService.CalcularArmaduraAsync(id);
            return armadura is null ? NotFound() : Ok(armadura);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/verificaPunzonado
        // Devuelve la verificación de punzonado en la columna
        // ================================================
        [HttpGet("{id}/verificaPunzonado")]
        public async Task<ActionResult<BaseHormigonVerificacionPunzonado>> GetVerificaPunzonado(long id)
        {
            var result = await _baseHormigonService.VerificarPunzonadoAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/verificaCorte
        // Devuelve la verificación de corte en la base
        // ================================================
        [HttpGet("{id}/verificaCorte")]
        public async Task<ActionResult<BaseHormigonVerificacionCorte>> GetVerificaCorte(long id)
        {
            var result = await _baseHormigonService.VerificarCorteAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        // ================================================
        // GET: api/baseshormigon/{id}/computo
        // Devuelve el cómputo de materiales (excavación, hormigón, acero, costos, etc.)
        // ================================================
        [HttpGet("{id}/computo")]
        public async Task<ActionResult<BaseHormigonComputo>> GetComputo(long id)
        {
            var computo = await _baseHormigonService.ComputoAsync(id);
            return computo is null ? NotFound() : Ok(computo);
        }

        // ================================================
        // POST: api/baseshormigon
        // Crea una nueva base
        // ================================================
        [HttpPost]
        public async Task<ActionResult<BaseHormigon>> PostBaseHormigon(BaseHormigon baseHormigon)
        {
            var created = await _baseHormigonService.CreateAsync(baseHormigon);
            return CreatedAtAction(nameof(GetBaseHormigon), new { id = created?.Id }, created);
        }

        // ================================================
        // PUT: api/baseshormigon/{id}
        // Actualiza una base existente
        // ================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBaseHormigon(long id, BaseHormigon baseHormigon)
        {
            var success = await _baseHormigonService.UpdateAsync(id, baseHormigon);
            return success ? NoContent() : BadRequest();
        }

        // ================================================
        // DELETE: api/baseshormigon/{id}
        // Elimina una base por ID
        // ================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBaseHormigon(long id)
        {
            var success = await _baseHormigonService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        // ============================================================
        // POST: api/baseshormigon/calculoArmadura/{id}
        // Calcula la armadura pero permitiendo enviar nuevos diámetros
        // de barras desde el frontend, útil para recalcular y comparar
        // ============================================================
        [HttpPost("calculoArmadura/{id}")]
        public async Task<ActionResult<BaseHormigonArmadura>> CalcularArmaduraCustom(
            long id,
            [FromBody] BaseHormigonDiametrosBarras nuevosDiametros)
        {
            var armadura = await _baseHormigonService.CalcularArmaduraConDiametrosAsync(id, nuevosDiametros);
            return armadura is null ? NotFound() : Ok(armadura);
        }
    }
}
