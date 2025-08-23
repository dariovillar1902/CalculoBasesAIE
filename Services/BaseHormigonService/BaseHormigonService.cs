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

        public async Task<BaseHormigonVerificaciones?> VerificarTensionAdmisibleAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);
            var verificaciones = VerificarTension(baseHormigon, dim);

            return verificaciones;
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
            // Fuerzas → kN
            if (baseHormigon.EsfuerzoAxil.Unidad == "N")
            {
                baseHormigon.EsfuerzoAxil.Valor /= 1000;
                baseHormigon.EsfuerzoAxil.Unidad = "kN";
            }
            else if (baseHormigon.EsfuerzoAxil.Unidad == "tf")
            {
                baseHormigon.EsfuerzoAxil.Valor *= 9.80665;
                baseHormigon.EsfuerzoAxil.Unidad = "kN";
            }

            // Presión / esfuerzo → kPa
            void ConvertPresion(ValueUnitPair campo)
            {
                if (campo.Unidad == "MPa")
                {
                    campo.Valor *= 1000;
                    campo.Unidad = "kPa";
                }
                else if (campo.Unidad == "Pa")
                {
                    campo.Valor /= 1000;
                    campo.Unidad = "kPa";
                }
                else if (campo.Unidad == "kg/cm²")
                {
                    campo.Valor *= 98.0665; // 1 kg/cm² = 98.0665 kPa
                    campo.Unidad = "kPa";
                }
            }

            ConvertPresion(baseHormigon.CargaAdmisible);
            ConvertPresion(baseHormigon.ResistenciaCaracteristicaHormigon);
            ConvertPresion(baseHormigon.TensionFluenciaAcero);

            // Porcentajes → %
            if (baseHormigon.PorcentajeCargaD.Unidad == "-")
            {
                baseHormigon.PorcentajeCargaD.Valor *= 100;
                baseHormigon.PorcentajeCargaD.Unidad = "%";
            }
            if (baseHormigon.PorcentajeCargaL.Unidad == "-")
            {
                baseHormigon.PorcentajeCargaL.Valor *= 100;
                baseHormigon.PorcentajeCargaL.Unidad = "%";
            }

            // Longitudes → m
            void ConvertLongitud(ValueUnitPair campo)
            {
                if (campo.Unidad == "cm")
                {
                    campo.Valor /= 100;
                    campo.Unidad = "m";
                }
                else if (campo.Unidad == "mm")
                {
                    campo.Valor /= 1000;
                    campo.Unidad = "m";
                }
            }

            ConvertLongitud(baseHormigon.AnchoColumnaX);
            ConvertLongitud(baseHormigon.AnchoColumnaY);
            ConvertLongitud(baseHormigon.NivelFundacion);
            ConvertLongitud(baseHormigon.RecubrimientoHormigon);
            ConvertLongitud(baseHormigon.DiametroBarrasX);
            ConvertLongitud(baseHormigon.DiametroBarrasY);

            // Densidades → kN/m³
            void ConvertDensidad(ValueUnitPair campo)
            {
                if (campo.Unidad == "N/m³")
                {
                    campo.Valor /= 1000;
                    campo.Unidad = "kN/m³";
                }
                else if (campo.Unidad == "kg/m³")
                {
                    campo.Valor *= 9.80665 / 1000; // convertir a kN/m³
                    campo.Unidad = "kN/m³";
                }
            }

            ConvertDensidad(baseHormigon.PesoEspecificoSuelo);
            ConvertDensidad(baseHormigon.PesoEspecificoHormigon);

            // Rigidez (módulo de balasto vertical) → kN/m³
            if (baseHormigon.ModuloBalastoVertical != null)
            {
                if (baseHormigon.ModuloBalastoVertical.Unidad == "MN/m³")
                {
                    baseHormigon.ModuloBalastoVertical.Valor *= 1000;
                    baseHormigon.ModuloBalastoVertical.Unidad = "kN/m³";
                }
            }
        }

        public BaseHormigonDimensiones EstimarDimensiones(BaseHormigon baseHormigon)
        {
            var dimensiones = new BaseHormigonDimensiones
            {
                // 1. Carga de diseño
                CargaDiseno = 1.10 * baseHormigon.EsfuerzoAxil.Valor,

                // 2. Tensión promedio admisible
                TensionPromedio = 0.65 * 1.25 * baseHormigon.CargaAdmisible.Valor
            };

            // 3. Área necesaria
            dimensiones.AreaNecesaria = dimensiones.CargaDiseno / dimensiones.TensionPromedio;

            // 4. Dimensiones base siguiendo relación AnchoX / AnchoY = 1.5
            dimensiones.RelacionLados = 1.5;
            dimensiones.AnchoY = Math.Sqrt(dimensiones.AreaNecesaria / dimensiones.RelacionLados);
            dimensiones.AnchoX = dimensiones.RelacionLados * dimensiones.AnchoY;

            // 5. Ajuste a múltiplos de 0.1 m
            dimensiones.AnchoX = Math.Ceiling(dimensiones.AnchoX * 10) / 10;
            dimensiones.AnchoY = Math.Ceiling(dimensiones.AnchoY * 10) / 10;

            // 6. Vuelos
            dimensiones.VueloX = dimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor;
            dimensiones.VueloY = dimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor;
            dimensiones.VerificaVuelos = Math.Abs(dimensiones.VueloX - dimensiones.VueloY) < 0.2;

            // 7. Altura mínima según relación de vuelo
            double[] alturas =
            [
        dimensiones.VueloX / 5,
        dimensiones.VueloY / 5,
        0.25 // altura mínima sugerida por norma
            ];
            dimensiones.Altura = Math.Ceiling(alturas.Max() * 20) / 20;

            // 8. Área final
            dimensiones.Area = dimensiones.AnchoX * dimensiones.AnchoY;

            return dimensiones;
        }

        public BaseHormigonVerificaciones VerificarTension(BaseHormigon baseHormigon, BaseHormigonDimensiones dimensiones)
        {
            // Excentricidades
            double ex = baseHormigon.MomentoX.Valor / baseHormigon.EsfuerzoAxil.Valor; // en metros
            double ey = baseHormigon.MomentoY.Valor / baseHormigon.EsfuerzoAxil.Valor; // en metros

            // Tensiones ajustadas por excentricidad
            double tensionX = dimensiones.CargaDiseno * (1 + 6 * ex / dimensiones.AnchoX) / (dimensiones.AnchoX * dimensiones.AnchoY);
            double tensionY = dimensiones.CargaDiseno * (1 - 6 * ey / dimensiones.AnchoY) / (dimensiones.AnchoX * dimensiones.AnchoY);

            // Verificación máxima
            bool dentroLimite = Math.Max(tensionX, tensionY) < dimensiones.TensionPromedio;

            return new BaseHormigonVerificaciones
            {
                TensionX = tensionX,
                TensionY = tensionY,
                VerificaTension = dentroLimite
            };
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
    }
}