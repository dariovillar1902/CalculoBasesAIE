using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalculoBasesAIE.Models;

namespace CalculoBasesAIE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesHormigonController(BaseHormigonContext context) : ControllerBase
    {
        private readonly BaseHormigonContext _context = context;

        // GET: api/BasesHormigon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BaseHormigon>>> GetBasesHormigon()
        {
            return await _context.BasesHormigon.ToListAsync();
        }

        // GET: api/BasesHormigon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseHormigon>> GetBaseHormigon(long id)
        {
            var baseHormigon = await _context.BasesHormigon.FindAsync(id);

            if (baseHormigon == null)
            {
                return NotFound();
            }

            return baseHormigon;
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

            _context.Entry(baseHormigon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificaTensionAdmisible = VerificarTension(baseHormigon, dimensionesBase);
            var calculoCuantia = CalcularCuantia(baseHormigon, dimensionesBase);
            var calculoArmadura = CalcularArmadura(baseHormigon, dimensionesBase, calculoCuantia);
            var verificaPunzonado = VerificarPunzonado(baseHormigon, dimensionesBase);
            var verificaCorte = VerificarCorte(baseHormigon, dimensionesBase);

            _context.BasesHormigon.Add(baseHormigon);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBaseHormigon), new { id = baseHormigon.Id }, baseHormigon);
        }

        // DELETE: api/BasesHormigon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBaseHormigon(long id)
        {
            var baseHormigon = await _context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null)
            {
                return NotFound();
            }

            _context.BasesHormigon.Remove(baseHormigon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BaseHormigonExists(long id)
        {
            return _context.BasesHormigon.Any(e => e.Id == id);
        }

        private static BaseHormigonDimensiones EstimarDimensiones(BaseHormigon baseHormigon)
        {
            var baseHormigonDimensiones = new BaseHormigonDimensiones
            {
                Area = 1.05 * baseHormigon.EsfuerzoAxil / (baseHormigon.CargaAdmisible - baseHormigon.PesoEspecificoSuelo * baseHormigon.NivelFundacion)
            };

            baseHormigonDimensiones.AnchoX = Math.Sqrt(baseHormigonDimensiones.Area + (Math.Pow(baseHormigon.AnchoColumnaX - baseHormigon.AnchoColumnaY, 2)) / 4) + (baseHormigon.AnchoColumnaX - baseHormigon.AnchoColumnaY) / 2;
            baseHormigonDimensiones.AnchoY = Math.Sqrt(baseHormigonDimensiones.Area + (Math.Pow(baseHormigon.AnchoColumnaX - baseHormigon.AnchoColumnaY, 2)) / 4) - (baseHormigon.AnchoColumnaX - baseHormigon.AnchoColumnaY) / 2;

            baseHormigonDimensiones.AnchoX = Math.Ceiling(baseHormigonDimensiones.AnchoX * 10) / 10;
            baseHormigonDimensiones.AnchoY = Math.Ceiling(baseHormigonDimensiones.AnchoY * 10) / 10;

            baseHormigonDimensiones.VueloX = baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX;
            baseHormigonDimensiones.VueloY = baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY;
            baseHormigonDimensiones.VerificaVuelos = Math.Abs(baseHormigonDimensiones.VueloX - baseHormigonDimensiones.VueloY) < 0.2;
            var condicionesAltura = new double[] { (baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX) / 5, (baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY) / 5, 0.25 };
            baseHormigonDimensiones.Altura = Math.Ceiling(condicionesAltura.Max() * 20) / 20;

            return baseHormigonDimensiones;
        }

        private static bool VerificarTension(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var cargaTotal = baseHormigon.EsfuerzoAxil / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) + baseHormigon.PesoEspecificoHormigon * baseHormigonDimensiones.Altura + baseHormigon.PesoEspecificoSuelo * (baseHormigon.NivelFundacion - baseHormigonDimensiones.Altura);

            return cargaTotal < baseHormigon.CargaAdmisible;
        }

        private static (double, double) CalcularCuantia(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var combinacionesCarga = new double[] { 1.4 * baseHormigon.PorcentajeCargaD * baseHormigon.EsfuerzoAxil, 1.2 * baseHormigon.PorcentajeCargaD * baseHormigon.EsfuerzoAxil + 1.6 * baseHormigon.PorcentajeCargaL * baseHormigon.EsfuerzoAxil };
            var esfuerzoAxilMayorado = combinacionesCarga.Max();

            var cargaMayorada = esfuerzoAxilMayorado / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);

            var momentoMayoradoX = cargaMayorada * Math.Pow(baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX, 2) * baseHormigonDimensiones.AnchoY / 8;
            var momentoMayoradoY = cargaMayorada * Math.Pow(baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY, 2) * baseHormigonDimensiones.AnchoX / 8;

            var momentoNominalX = momentoMayoradoX / 0.9;
            var momentoNominalY = momentoMayoradoY / 0.9;

            var factorAdimensionalX = momentoNominalX / (baseHormigonDimensiones.AnchoY * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02), 2) * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon * 1000);
            var factorAdimensionalY = momentoNominalY / (baseHormigonDimensiones.AnchoX * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02), 2) * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon * 1000);

            var cuantiaMecanicaX = 1 - Math.Sqrt(1 - 2 * factorAdimensionalX);
            var cuantiaMecanicaY = 1 - Math.Sqrt(1 - 2 * factorAdimensionalY);

            var cuantiaCalculoX = cuantiaMecanicaX * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon / baseHormigon.TensionFluenciaAcero;
            var cuantiaCalculoY = cuantiaMecanicaY * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon / baseHormigon.TensionFluenciaAcero;

            var cuantiaMaxima = 0.85 * 3 * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon / (8 * baseHormigon.TensionFluenciaAcero);

            var verificaCuantiaMaxima = cuantiaCalculoX < cuantiaMaxima && cuantiaCalculoY < cuantiaMaxima;

            var cuantiaMinima = 1.4 / baseHormigon.TensionFluenciaAcero;

            var cuantiaAdoptadaX = Math.Min(Math.Max(cuantiaCalculoX, cuantiaMinima), 4 / 3.0 * cuantiaCalculoX);
            var cuantiaAdoptadaY = Math.Min(Math.Max(cuantiaCalculoY, cuantiaMinima), 4 / 3.0 * cuantiaCalculoY);

            var areaAceroX = cuantiaAdoptadaX * baseHormigonDimensiones.AnchoY * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02);
            var areaAceroY = cuantiaAdoptadaY * baseHormigonDimensiones.AnchoX * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02);
        
            return (areaAceroX, areaAceroY);
        }

        private static BaseHormigonArmadura CalcularArmadura(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, (double, double) areasAdoptadas)
        {
            var diametrosBarras = (0.016, 0.016);

            var areasBarrasIndividuales = (Math.PI * Math.Pow(diametrosBarras.Item1, 2) / 4, Math.PI * Math.Pow(diametrosBarras.Item2, 2) / 4);
            
            var cantidadBarras = (
                Math.Ceiling(areasAdoptadas.Item1 / areasBarrasIndividuales.Item1),
                Math.Ceiling(areasAdoptadas.Item2 / areasBarrasIndividuales.Item2)
            );

            var separacionBarras = (
                Math.Floor(Math.Min((baseHormigonDimensiones.AnchoY - 2 * baseHormigon.RecubrimientoHormigon) / (cantidadBarras.Item1 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item1, 0.3 }.Min()) * 100) / 100,

                Math.Floor(Math.Min((baseHormigonDimensiones.AnchoX - 2 * baseHormigon.RecubrimientoHormigon) / (cantidadBarras.Item2 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item2, 0.3 }.Min()) * 100) / 100
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

        private static bool VerificarPunzonado(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var combinacionesCarga = new double[] { 1.4 * baseHormigon.PorcentajeCargaD * baseHormigon.EsfuerzoAxil, 1.2 * baseHormigon.PorcentajeCargaD * baseHormigon.EsfuerzoAxil + 1.6 * baseHormigon.PorcentajeCargaL * baseHormigon.EsfuerzoAxil };
            var esfuerzoAxilMayorado = combinacionesCarga.Max();

            var cargaTotal = baseHormigon.EsfuerzoAxil / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) + baseHormigon.PesoEspecificoHormigon * baseHormigonDimensiones.Altura + baseHormigon.PesoEspecificoSuelo * (baseHormigon.NivelFundacion - baseHormigonDimensiones.Altura);

            var resistenciaRequerida = esfuerzoAxilMayorado - cargaTotal * (baseHormigon.AnchoColumnaX + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02)) * (baseHormigon.AnchoColumnaY + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02));

            var b0 = 2 * (baseHormigon.AnchoColumnaX + baseHormigon.AnchoColumnaY) + 4 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02);

            var b = new double[] { baseHormigon.AnchoColumnaX, baseHormigon.AnchoColumnaY }.Max() / new double[] { baseHormigon.AnchoColumnaX, baseHormigon.AnchoColumnaY }.Min();

            var resistenciasNominales = new double[] { b0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon * 1000 * 1000) / 3, (1 + 2 / b) * b0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon * 1000 * 1000) / 6, (2 + 40 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02) / b0) * b0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon * 1000 * 1000) / 12 };

            var resistenciaNominal = resistenciasNominales.Min();

            var resistenciaDiseno = resistenciaNominal * 0.75;

            var verificaResistenciaPunzonado = resistenciaRequerida <= resistenciaDiseno;

            return verificaResistenciaPunzonado;
        }

        private static bool VerificarCorte(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var cargaTotal = baseHormigon.EsfuerzoAxil / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) + baseHormigon.PesoEspecificoHormigon * baseHormigonDimensiones.Altura + baseHormigon.PesoEspecificoSuelo * (baseHormigon.NivelFundacion - baseHormigonDimensiones.Altura);

            var resistenciaRequeridaX = cargaTotal * (((baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX) / 2 - (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02))) * baseHormigonDimensiones.AnchoY;
            var resistenciaRequeridaY = cargaTotal * (((baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY) / 2 - (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02))) * baseHormigonDimensiones.AnchoX;

            var resistenciaNominalX = baseHormigonDimensiones.AnchoY * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon * 1000 * 1000) / 6;
            var resistenciaNominalY = baseHormigonDimensiones.AnchoX * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon * 1000 * 1000) / 6;

            var resistenciaDisenoX = resistenciaNominalX * 0.75;
            var resistenciaDisenoY = resistenciaNominalY * 0.75;

            var verificaResistenciaCorte = resistenciaRequeridaX <= resistenciaDisenoX && resistenciaRequeridaY < resistenciaDisenoY; 

            return verificaResistenciaCorte;
        }
    }
}
