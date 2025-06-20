using Microsoft.AspNetCore.Mvc;
using CalculoBasesAIE.Models;
using CalculoBasesAIE.Services.BaseHormigonService;

namespace CalculoBasesAIE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesHormigonController(BaseHormigonService baseHormigonService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BaseHormigon>>> GetBasesHormigon()
        {
            var items = await baseHormigonService.GetAllBasesAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseHormigon>> GetBaseHormigon(long id)
        {
            var item = await baseHormigonService.GetBaseByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("{id}/dimensionesBase")]
        public async Task<ActionResult<BaseHormigonDimensiones>> GetDimensionesBase(long id)
        {
            var dim = await baseHormigonService.GetDimensionesAsync(id);
            return dim is null ? NotFound() : Ok(dim);
        }

        [HttpGet("{id}/verificaTensionAdmisible")]
        public async Task<ActionResult<bool>> GetVerificaTensionAdmisible(long id)
        {
            var result = await baseHormigonService.VerificarTensionAdmisibleAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("{id}/calculoCuantia")]
        public async Task<ActionResult<BaseHormigonCuantia>> GetCalculoCuantia(long id)
        {
            var cuantia = await baseHormigonService.CalcularCuantiaAsync(id);
            return cuantia is null ? NotFound() : Ok(cuantia);
        }

        [HttpGet("{id}/calculoArmadura")]
        public async Task<ActionResult<BaseHormigonArmadura>> GetCalculoArmadura(long id)
        {
            var armadura = await baseHormigonService.CalcularArmaduraAsync(id);
            return armadura is null ? NotFound() : Ok(armadura);
        }

        [HttpGet("{id}/verificaPunzonado")]
        public async Task<ActionResult<BaseHormigonVerificacionPunzonado>> GetVerificaPunzonado(long id)
        {
            var result = await baseHormigonService.VerificarPunzonadoAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("{id}/verificaCorte")]
        public async Task<ActionResult<BaseHormigonVerificacionCorte>> GetVerificaCorte(long id)
        {
            var result = await baseHormigonService.VerificarCorteAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BaseHormigon>> PostBaseHormigon(BaseHormigon baseHormigon)
        {
            var created = await baseHormigonService.CreateAsync(baseHormigon);
            return CreatedAtAction(nameof(GetBaseHormigon), new { id = created?.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBaseHormigon(long id, BaseHormigon baseHormigon)
        {
            var success = await baseHormigonService.UpdateAsync(id, baseHormigon);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBaseHormigon(long id)
        {
            var success = await baseHormigonService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("calculoArmadura/{id}")]
        public async Task<ActionResult<BaseHormigonArmadura>> CalcularArmaduraCustom(long id, [FromBody] BaseHormigonDiametrosBarras nuevosDiametros)
        {
            var armadura = await baseHormigonService.CalcularArmaduraConDiametrosAsync(id, nuevosDiametros);
            return armadura is null ? NotFound() : Ok(armadura);
        }
    }
}
