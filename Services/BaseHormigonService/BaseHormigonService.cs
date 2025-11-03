using CalculoBasesAIE.Models;
using CalculoBasesAIE.Repositories.BaseHormigonRepository;

namespace CalculoBasesAIE.Services.BaseHormigonService
{
    public class BaseHormigonService(IBaseHormigonRepository repository) : IBaseHormigonService
    {
        public async Task<List<BaseHormigon>> GetAllBasesAsync() =>
            await repository.GetAllAsync();

        public async Task<BaseHormigon?> GetBaseByIdAsync(long id) =>
            await repository.GetByIdAsync(id);

        public async Task<BaseHormigonDimensiones?> GetDimensionesAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            return baseHormigon == null ? null : EstimarDimensiones(baseHormigon);
        }

        public async Task<BaseHormigonEsfuerzos?> GetEsfuerzosAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            return ObtenerEsfuerzos(baseHormigon, dim);
        }

        public async Task<BaseHormigonVerificaciones?> VerificarBaseAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            var esfuerzos = ObtenerEsfuerzos(baseHormigon, dim);
            return VerificarBase(baseHormigon, dim, esfuerzos);
        }

        public async Task<BaseHormigonCuantia?> CalcularCuantiaAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            return CalcularCuantia(baseHormigon, dim);
        }

        public async Task<BaseHormigonArmadura?> CalcularArmaduraAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);
            return CalcularArmadura(baseHormigon, dim, cuantia);
        }

        public async Task<BaseHormigonComputo?> ComputoAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);
            var armadura = CalcularArmadura(baseHormigon, dim, cuantia);
            return Computo(baseHormigon, dim, armadura);
        }

        public async Task<BaseHormigonVerificacionPunzonado?> VerificarPunzonadoAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            return VerificarPunzonado(baseHormigon, dim);
        }

        public async Task<BaseHormigonVerificacionCorte?> VerificarCorteAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;
            var dim = EstimarDimensiones(baseHormigon);
            return VerificarCorte(baseHormigon, dim);
        }

        public async Task<BaseHormigon?> CreateAsync(BaseHormigon baseHormigon)
        {
            ConvertirUnidades(baseHormigon);
            var baseHormigonExistente = await repository.GetDuplicateAsync(baseHormigon);

            if (baseHormigonExistente != null) {
                return baseHormigonExistente;
            } else {
                await repository.AddAsync(baseHormigon);
                return baseHormigon;
            }
        }

        public async Task<bool> UpdateAsync(long id, BaseHormigon baseHormigon)
        {
            if (!await repository.ExistsAsync(id)) return false;
            baseHormigon.Id = id;
            await repository.UpdateAsync(baseHormigon);
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null) return false;

            await repository.DeleteAsync(entity);
            return true;
        }

        public async Task<BaseHormigonArmadura?> CalcularArmaduraConDiametrosAsync(long id, BaseHormigonDiametrosBarras diametros)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            baseHormigon.DiametroBarrasX.Valor = diametros.DiametroX / 1000.0;
            baseHormigon.DiametroBarrasY.Valor = diametros.DiametroY / 1000.0;

            // Optional: Normalize diameter units
            baseHormigon.DiametroBarrasX.Unidad = "m";
            baseHormigon.DiametroBarrasY.Unidad = "m";

            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);
            return CalcularArmadura(baseHormigon, dim, cuantia);
        }

        public void ConvertirUnidades(BaseHormigon baseHormigon)
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
            }
            else if (baseHormigon.CargaAdmisible.Unidad == "Pa")
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

        public BaseHormigonDimensiones EstimarDimensiones(BaseHormigon baseHormigon)
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

        public BaseHormigonEsfuerzos ObtenerEsfuerzos(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var baseHormigonEsfuerzos = new BaseHormigonEsfuerzos
            {
                Normal = baseHormigon.EsfuerzoAxil.Valor +
                baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.Altura * baseHormigon.PesoEspecificoHormigon.Valor +
                baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura) * baseHormigon.PesoEspecificoSuelo.Valor,
                
                MomentoX = baseHormigon.MomentoX.Valor + baseHormigon.CorteX.Valor * baseHormigonDimensiones.Altura,
                MomentoY = baseHormigon.MomentoY.Valor + baseHormigon.CorteY.Valor * baseHormigonDimensiones.Altura,

                CorteX = baseHormigon.CorteX.Valor,
                CorteY = baseHormigon.CorteY.Valor
            };

            return baseHormigonEsfuerzos;
        }

        public BaseHormigonVerificaciones VerificarBase(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonEsfuerzos baseHormigonEsfuerzos)
        {
            var baseHormigonVerificaciones = new BaseHormigonVerificaciones
            {
                CoeficienteSeguridadVuelco = Math.Min((baseHormigonEsfuerzos.Normal * baseHormigonDimensiones.AnchoX / 2) / baseHormigonEsfuerzos.MomentoX,
                baseHormigonEsfuerzos.Normal * baseHormigonDimensiones.AnchoY / 2) / baseHormigonEsfuerzos.MomentoY
            };

            baseHormigonVerificaciones.VerificaVuelco = baseHormigonVerificaciones.CoeficienteSeguridadVuelco >= 1.5;

            baseHormigonVerificaciones.ExcentricidadX = baseHormigonEsfuerzos.MomentoX / baseHormigonEsfuerzos.Normal;
            baseHormigonVerificaciones.ExcentricidadY = baseHormigonEsfuerzos.MomentoY / baseHormigonEsfuerzos.Normal;

            if (baseHormigonVerificaciones.ExcentricidadX == 0)
            {
                baseHormigonVerificaciones.TensionMaximaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
                baseHormigonVerificaciones.TensionMinimaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
            } else if (baseHormigonVerificaciones.ExcentricidadX <= baseHormigonDimensiones.AnchoX / 6)
            {
                baseHormigonVerificaciones.TensionMaximaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) *
                    (1 + 6 * baseHormigonVerificaciones.ExcentricidadX / baseHormigonDimensiones.AnchoX);
                baseHormigonVerificaciones.TensionMinimaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) *
                    (1 - 6 * baseHormigonVerificaciones.ExcentricidadX / baseHormigonDimensiones.AnchoX);
            } else if (baseHormigonVerificaciones.ExcentricidadX > baseHormigonDimensiones.AnchoX / 6)
            {
                baseHormigonVerificaciones.TensionMaximaX = 4 * baseHormigonEsfuerzos.Normal / (3 * (baseHormigonDimensiones.AnchoX - 2 * baseHormigonVerificaciones.ExcentricidadX) * baseHormigonDimensiones.AnchoY);
                baseHormigonVerificaciones.TensionMinimaX = 0;
            }

            if (baseHormigonVerificaciones.ExcentricidadY == 0)
            {
                baseHormigonVerificaciones.TensionMaximaY = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
                baseHormigonVerificaciones.TensionMinimaY = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
            }
            else if (baseHormigonVerificaciones.ExcentricidadY <= baseHormigonDimensiones.AnchoY / 6)
            {
                baseHormigonVerificaciones.TensionMaximaY = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) *
                    (1 + 6 * baseHormigonVerificaciones.ExcentricidadY / baseHormigonDimensiones.AnchoY);
                baseHormigonVerificaciones.TensionMinimaY = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) *
                    (1 - 6 * baseHormigonVerificaciones.ExcentricidadY / baseHormigonDimensiones.AnchoY);
            }
            else if (baseHormigonVerificaciones.ExcentricidadY > baseHormigonDimensiones.AnchoY / 6)
            {
                baseHormigonVerificaciones.TensionMaximaY = 4 * baseHormigonEsfuerzos.Normal / (3 * (baseHormigonDimensiones.AnchoY - 2 * baseHormigonVerificaciones.ExcentricidadY) * baseHormigonDimensiones.AnchoX);
                baseHormigonVerificaciones.TensionMinimaY = 0;
            }

            baseHormigonVerificaciones.VerificaTensionAdmisible = baseHormigonVerificaciones.TensionMaximaX <= 1.25 * baseHormigon.CargaAdmisible.Valor &&
                baseHormigonVerificaciones.TensionMaximaY <= 1.25 * baseHormigon.CargaAdmisible.Valor &&
                (baseHormigonVerificaciones.TensionMaximaX + baseHormigonVerificaciones.TensionMinimaX) / 2 <= baseHormigon.CargaAdmisible.Valor &&
                (baseHormigonVerificaciones.TensionMaximaY + baseHormigonVerificaciones.TensionMinimaY) / 2 <= baseHormigon.CargaAdmisible.Valor;

            baseHormigonVerificaciones.AsentamientoMedio = baseHormigonEsfuerzos.Normal / (baseHormigon.ModuloBalasto.Valor * baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
            baseHormigonVerificaciones.AsentamientoMaximo = (1 / baseHormigon.ModuloBalasto.Valor) *
                (baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                6 * baseHormigon.MomentoX.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoY) +
                6 * baseHormigon.MomentoY.Valor / (baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoX));

            baseHormigonVerificaciones.AsentamientoMinimo = (1 / baseHormigon.ModuloBalasto.Valor) *
                (baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) -
                6 * baseHormigon.MomentoX.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoY) -
                6 * baseHormigon.MomentoY.Valor / (baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoX));

            baseHormigonVerificaciones.VerificaAsentamientoMedio = baseHormigonVerificaciones.AsentamientoMedio <= 0.035;

            baseHormigonVerificaciones.DistorsionAngular = (baseHormigonVerificaciones.AsentamientoMaximo - baseHormigonVerificaciones.AsentamientoMinimo) /
                Math.Sqrt(Math.Pow(baseHormigonDimensiones.AnchoX, 2) + Math.Pow(baseHormigonDimensiones.AnchoY, 2));

            baseHormigonVerificaciones.VerificaAsentamientoDiferencial = baseHormigonVerificaciones.DistorsionAngular <= 0.002;

            return baseHormigonVerificaciones;
        }


        public bool VerificarTension(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var cargaTotal = baseHormigon.EsfuerzoAxil.Valor /
                             (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            return cargaTotal < baseHormigon.CargaAdmisible.Valor;
        }

        public BaseHormigonCuantia CalcularCuantia(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
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

        public BaseHormigonArmadura CalcularArmadura(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonCuantia baseHormigonCuantia)
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

        public BaseHormigonVerificacionPunzonado VerificarPunzonado(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
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

        public BaseHormigonVerificacionCorte VerificarCorte(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
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

        public BaseHormigonComputo Computo(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonArmadura baseHormigonArmadura)
        {
            var volumenHormigon = baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.Altura;

            var longitudBarrasX = baseHormigonArmadura.CantidadBarrasX * (baseHormigonDimensiones.AnchoX + 0.5);
            var longitudBarrasY = baseHormigonArmadura.CantidadBarrasY * (baseHormigonDimensiones.AnchoY + 0.5);

            var diametrosBarras = (baseHormigon.DiametroBarrasX.Valor, baseHormigon.DiametroBarrasY.Valor);

            var areasBarrasIndividuales = (Math.PI * Math.Pow(diametrosBarras.Item1, 2) / 4, Math.PI * Math.Pow(diametrosBarras.Item2, 2) / 4);

            var pesoBarrasX = longitudBarrasX * areasBarrasIndividuales.Item1 * 7850;
            var pesoBarrasY = longitudBarrasY * areasBarrasIndividuales.Item2 * 7850;

            var volumenExcavacion = volumenHormigon * baseHormigon.CoeficienteEsponjamiento.Valor;

            var costoHormigon = volumenHormigon * baseHormigon.CostoM3Hormigon.Valor;
            var costoAcero = (pesoBarrasX + pesoBarrasY) * baseHormigon.CostoKgAcero.Valor;
            var costoExcavacion = volumenExcavacion * baseHormigon.CostoM3Excavacion.Valor;

            var baseHormigonComputo = new BaseHormigonComputo()
            {
                VolumenHormigon = volumenHormigon,
                LongitudBarrasX = longitudBarrasX,
                LongitudBarrasY = longitudBarrasY,
                PesoBarrasX = pesoBarrasX,
                PesoBarrasY = pesoBarrasY,
                VolumenExcavacion = volumenExcavacion,
                MontoHormigon = costoHormigon,
                MontoAcero = costoAcero,
                MontoExcavacion = costoExcavacion
            };

            return baseHormigonComputo;
        }
    }
}