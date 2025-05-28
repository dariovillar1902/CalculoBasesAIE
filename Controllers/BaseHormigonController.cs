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

        // Get estimated dimensions
        [HttpGet("{id}/dimensionesBase")]
        public async Task<ActionResult<BaseHormigonDimensiones>> GetDimensionesBase(long id)
        {
            var baseHormigon = await _context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            return Ok(dimensionesBase);
        }

        // Get tension verification
        [HttpGet("{id}/verificaTensionAdmisible")]
        public async Task<ActionResult<bool>> GetVerificaTensionAdmisible(long id)
        {
            var baseHormigon = await _context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var verificaTension = VerificarTension(baseHormigon, dimensionesBase);
            return Ok(verificaTension);
        }

        // Get reinforcement calculations
        [HttpGet("{id}/calculoCuantia")]
        public async Task<ActionResult<(double, double)>> GetCalculoCuantia(long id)
        {
            var baseHormigon = await _context.BasesHormigon.FindAsync(id);
            if (baseHormigon == null) return NotFound();

            var dimensionesBase = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dimensionesBase);
            return Ok(cuantia);
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
            ConvertirUnidades(baseHormigon);
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


        private static (double, double) CalcularCuantia(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var combinacionesCarga = new double[]
            {
        1.4 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor,
        1.2 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor +
        1.6 * (baseHormigon.PorcentajeCargaL.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor
            };

            var esfuerzoAxilMayorado = combinacionesCarga.Max();
            var cargaMayorada = esfuerzoAxilMayorado / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);

            var momentoMayoradoX = cargaMayorada * Math.Pow(baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor, 2) * baseHormigonDimensiones.AnchoY / 8;
            var momentoMayoradoY = cargaMayorada * Math.Pow(baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor, 2) * baseHormigonDimensiones.AnchoX / 8;

            var momentoNominalX = momentoMayoradoX / 0.9;
            var momentoNominalY = momentoMayoradoY / 0.9;

            var factorAdimensionalX = momentoNominalX / (baseHormigonDimensiones.AnchoY * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02), 2) * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000);
            var factorAdimensionalY = momentoNominalY / (baseHormigonDimensiones.AnchoX * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02), 2) * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000);

            var cuantiaMecanicaX = 1 - Math.Sqrt(1 - 2 * factorAdimensionalX);
            var cuantiaMecanicaY = 1 - Math.Sqrt(1 - 2 * factorAdimensionalY);

            var cuantiaCalculoX = cuantiaMecanicaX * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor / (baseHormigon.TensionFluenciaAcero.Valor / 1000);
            var cuantiaCalculoY = cuantiaMecanicaY * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor / (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            var cuantiaMaxima = 0.85 * 3 * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor / (8 * (baseHormigon.TensionFluenciaAcero.Valor / 1000));
            var verificaCuantiaMaxima = cuantiaCalculoX < cuantiaMaxima && cuantiaCalculoY < cuantiaMaxima;

            var cuantiaMinima = 1.4 / (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            var cuantiaAdoptadaX = Math.Min(Math.Max(cuantiaCalculoX, cuantiaMinima), 4 / 3.0 * cuantiaCalculoX);
            var cuantiaAdoptadaY = Math.Min(Math.Max(cuantiaCalculoY, cuantiaMinima), 4 / 3.0 * cuantiaCalculoY);

            var areaAceroX = cuantiaAdoptadaX * baseHormigonDimensiones.AnchoY * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02);
            var areaAceroY = cuantiaAdoptadaY * baseHormigonDimensiones.AnchoX * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02);

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
                Math.Floor(Math.Min((baseHormigonDimensiones.AnchoY - 2 * baseHormigon.RecubrimientoHormigon.Valor) / (cantidadBarras.Item1 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item1, 0.3 }.Min()) * 100) / 100,

                Math.Floor(Math.Min((baseHormigonDimensiones.AnchoX - 2 * baseHormigon.RecubrimientoHormigon.Valor) / (cantidadBarras.Item2 - 1),
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
            var combinacionesCarga = new double[]
            {
        1.4 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor,
        1.2 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor +
        1.6 * (baseHormigon.PorcentajeCargaL.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor
            };

            var esfuerzoAxilMayorado = combinacionesCarga.Max();

            var cargaTotal = baseHormigon.EsfuerzoAxil.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            var resistenciaRequerida = esfuerzoAxilMayorado - cargaTotal * (baseHormigon.AnchoColumnaX.Valor + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02)) *
                                       (baseHormigon.AnchoColumnaY.Valor + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02));

            var b0 = 2 * (baseHormigon.AnchoColumnaX.Valor + baseHormigon.AnchoColumnaY.Valor) +
                     4 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02);

            var b = new double[] { baseHormigon.AnchoColumnaX.Valor, baseHormigon.AnchoColumnaY.Valor }.Max() /
                    new double[] { baseHormigon.AnchoColumnaX.Valor, baseHormigon.AnchoColumnaY.Valor }.Min();

            var resistenciasNominales = new double[]
            {
        b0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 3,
        (1 + 2 / b) * b0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) * Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6,
        (2 + 40 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) / b0) * b0 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
        Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 12
            };

            var resistenciaNominal = resistenciasNominales.Min();
            var resistenciaDiseno = resistenciaNominal * 0.75;

            return resistenciaRequerida <= resistenciaDiseno;
        }


        private static bool VerificarCorte(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var cargaTotal = baseHormigon.EsfuerzoAxil.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            var resistenciaRequeridaX = cargaTotal * (((baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor) / 2 -
                                      (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02))) * baseHormigonDimensiones.AnchoY;

            var resistenciaRequeridaY = cargaTotal * (((baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor) / 2 -
                                      (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02))) * baseHormigonDimensiones.AnchoX;

            var resistenciaNominalX = baseHormigonDimensiones.AnchoY * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
                                      Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6;

            var resistenciaNominalY = baseHormigonDimensiones.AnchoX * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
                                      Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6;

            var resistenciaDisenoX = resistenciaNominalX * 0.75;
            var resistenciaDisenoY = resistenciaNominalY * 0.75;

            return resistenciaRequeridaX <= resistenciaDisenoX && resistenciaRequeridaY < resistenciaDisenoY;
        }

    }
}
