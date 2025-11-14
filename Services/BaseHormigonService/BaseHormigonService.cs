using CalculoBasesAIE.Models;
using CalculoBasesAIE.Repositories.BaseHormigonRepository;

namespace CalculoBasesAIE.Services.BaseHormigonService
{
    public class BaseHormigonService(IBaseHormigonRepository repository) : IBaseHormigonService
    {
        public async Task<List<BaseHormigon>> GetAllBasesAsync() =>
    // Obtener todas las bases almacenadas
    await repository.GetAllAsync();

        public async Task<BaseHormigon?> GetBaseByIdAsync(long id) =>
            // Obtener una base por ID
            await repository.GetByIdAsync(id);

        public async Task<BaseHormigonDimensiones?> GetDimensionesAsync(long id)
        {
            // Buscar la base por ID
            var baseHormigon = await repository.GetByIdAsync(id);
            // Si no existe, devolver null
            return baseHormigon == null ? null : EstimarDimensiones(baseHormigon);
        }

        public async Task<BaseHormigonEsfuerzos?> GetEsfuerzosAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            // Las dimensiones dependen del modelo base
            var dim = EstimarDimensiones(baseHormigon);

            // Calcular esfuerzos en la base
            return ObtenerEsfuerzos(baseHormigon, dim);
        }

        public async Task<BaseHormigonVerificaciones?> VerificarBaseAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);
            var esfuerzos = ObtenerEsfuerzos(baseHormigon, dim);

            // Ejecutar todas las verificaciones principales
            return VerificarBase(baseHormigon, dim, esfuerzos);
        }

        public async Task<BaseHormigonCuantia?> CalcularCuantiaAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);

            // Calcular la cuantía mínima, requerida y adoptada
            return CalcularCuantia(baseHormigon, dim);
        }

        public async Task<BaseHormigonArmadura?> CalcularArmaduraAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);

            // Calcular armadura adoptada usando cuantía
            return CalcularArmadura(baseHormigon, dim, cuantia);
        }

        public async Task<BaseHormigonComputo?> ComputoAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);
            var armadura = CalcularArmadura(baseHormigon, dim, cuantia);

            // Generar cómputo de materiales (volumen, acero, etc.)
            return Computo(baseHormigon, dim, armadura);
        }

        public async Task<BaseHormigonVerificacionPunzonado?> VerificarPunzonadoAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);

            // Verificación al punzonado según CIRSOC/Eurocode
            return VerificarPunzonado(baseHormigon, dim);
        }

        public async Task<BaseHormigonVerificacionCorte?> VerificarCorteAsync(long id)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            var dim = EstimarDimensiones(baseHormigon);

            // Verificación por corte (resistencia a esfuerzos cortantes)
            return VerificarCorte(baseHormigon, dim);
        }

        public async Task<BaseHormigon?> CreateAsync(BaseHormigon baseHormigon)
        {
            // Convertir unidades al sistema interno (m, kN, etc.)
            ConvertirUnidades(baseHormigon);

            // Verificar si ya existe una base idéntica
            var baseHormigonExistente = await repository.GetDuplicateAsync(baseHormigon);

            if (baseHormigonExistente != null)
            {
                // Evita duplicados devolviendo la base ya existente
                return baseHormigonExistente;
            }
            else
            {
                // Guardar nueva base
                await repository.AddAsync(baseHormigon);
                return baseHormigon;
            }
        }

        public async Task<bool> UpdateAsync(long id, BaseHormigon baseHormigon)
        {
            // Comprobar si existe antes de actualizar
            if (!await repository.ExistsAsync(id)) return false;

            // Mantener ID estable
            baseHormigon.Id = id;

            // Actualizar entidad completa
            await repository.UpdateAsync(baseHormigon);
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            // Buscar entidad antes de eliminar
            var entity = await repository.GetByIdAsync(id);
            if (entity == null) return false;

            await repository.DeleteAsync(entity);
            return true;
        }

        public async Task<BaseHormigonArmadura?> CalcularArmaduraConDiametrosAsync(long id, BaseHormigonDiametrosBarras diametros)
        {
            var baseHormigon = await repository.GetByIdAsync(id);
            if (baseHormigon == null) return null;

            // Actualizar diámetro de barras desde el input (convertir mm → m)
            baseHormigon.DiametroBarrasX.Valor = diametros.DiametroX / 1000.0;
            baseHormigon.DiametroBarrasY.Valor = diametros.DiametroY / 1000.0;

            // Normalizar la unidad para evitar inconsistencias
            baseHormigon.DiametroBarrasX.Unidad = "m";
            baseHormigon.DiametroBarrasY.Unidad = "m";

            var dim = EstimarDimensiones(baseHormigon);
            var cuantia = CalcularCuantia(baseHormigon, dim);

            // Calcular armadura usando los diámetros personalizados
            return CalcularArmadura(baseHormigon, dim, cuantia);
        }


        public void ConvertirUnidades(BaseHormigon b)
        {
            Convert(b.EsfuerzoAxil, new()
            {
                { "N",  ("kN", v => v / 1000) }
            });

            Convert(b.CargaAdmisible, new()
            {
                { "MPa", ("kPa", v => v * 1000) },
                { "Pa",  ("kPa", v => v / 1000) }
            });

            Convert(b.PorcentajeCargaD, new()
            {
                { "-", ("%", v => v * 100) }
            });

            Convert(b.PorcentajeCargaL, new()
            {
                { "-", ("%", v => v * 100) }
            });

            Convert(b.AnchoColumnaX, new()
            {
                { "cm", ("m", v => v / 100) }
            });

            Convert(b.AnchoColumnaY, new()
            {
                { "cm", ("m", v => v / 100) }
            });

            Convert(b.DiametroBarrasX, new()
            {
                { "mm", ("m", v => v / 1000) },
                { "cm", ("m", v => v / 100) }
            });

            Convert(b.DiametroBarrasY, new()
            {
                { "mm", ("m", v => v / 1000) },
                { "cm", ("m", v => v / 100) }
            });

            Convert(b.PesoEspecificoSuelo, new()
            {
                { "N/m3", ("kN/m3", v => v / 1000) }
            });

            Convert(b.NivelFundacion, new()
            {
                { "cm", ("m", v => v / 100) }
            });

            Convert(b.PesoEspecificoHormigon, new()
            {
                { "N/m3", ("kN/m3", v => v / 1000) }
            });

            Convert(b.ResistenciaCaracteristicaHormigon, new()
            {
                { "MPa", ("kPa", v => v * 1000) },
                { "Pa",  ("kPa", v => v / 1000) }
            });

            Convert(b.RecubrimientoHormigon, new()
            {
                { "cm", ("m", v => v / 100) }
            });

            Convert(b.TensionFluenciaAcero, new()
            {
                { "MPa", ("kPa", v => v * 1000) },
                { "Pa",  ("kPa", v => v / 1000) }
            });
        }


        private static void Convert(ValueUnitPair v, Dictionary<string, (string unidadDestino, Func<double, double> conversion)> reglas)
        {
            if (v == null || string.IsNullOrEmpty(v.Unidad))
                return; // No hay nada que convertir

            if (reglas.TryGetValue(v.Unidad, out var regla))
            {
                v.Valor = regla.conversion(v.Valor);
                v.Unidad = regla.unidadDestino;
            }
        }



        // Método que estima las dimensiones de una base de hormigón a partir de sus datos principales
        public BaseHormigonDimensiones EstimarDimensiones(BaseHormigon baseHormigon)
        {
            // Creamos un objeto donde vamos a almacenar las dimensiones calculadas
            var baseHormigonDimensiones = new BaseHormigonDimensiones
            {
                // Cálculo del área de la base usando el esfuerzo axial y la carga admisible del suelo
                Area = 1.05 * baseHormigon.EsfuerzoAxil.Valor /
                       (baseHormigon.CargaAdmisible.Valor - baseHormigon.PesoEspecificoSuelo.Valor * baseHormigon.NivelFundacion.Valor)
            };

            // Cálculo del ancho en la dirección X considerando la diferencia entre los lados de la columna
            baseHormigonDimensiones.AnchoX = Math.Sqrt(baseHormigonDimensiones.Area +
                    (Math.Pow(baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor, 2)) / 4)
                    + (baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor) / 2;

            // Cálculo del ancho en la dirección Y considerando la diferencia entre los lados de la columna
            baseHormigonDimensiones.AnchoY = Math.Sqrt(baseHormigonDimensiones.Area +
                    (Math.Pow(baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor, 2)) / 4)
                    - (baseHormigon.AnchoColumnaX.Valor - baseHormigon.AnchoColumnaY.Valor) / 2;

            // Redondeo de los anchos a un decimal para simplificar las medidas
            baseHormigonDimensiones.AnchoX = Math.Ceiling(baseHormigonDimensiones.AnchoX * 10) / 10;
            baseHormigonDimensiones.AnchoY = Math.Ceiling(baseHormigonDimensiones.AnchoY * 10) / 10;

            // Cálculo del vuelo de la base en X e Y (proyección de la base más allá de la columna)
            baseHormigonDimensiones.VueloX = baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor;
            baseHormigonDimensiones.VueloY = baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor;

            // Verificación si los vuelos en X e Y son prácticamente iguales (tolerancia de 0.2 metros)
            baseHormigonDimensiones.VerificaVuelos = Math.Abs(baseHormigonDimensiones.VueloX - baseHormigonDimensiones.VueloY) < 0.2;

            // Condiciones para calcular la altura de la base
            var condicionesAltura = new double[]
            {
        // Proporción basada en el vuelo en X
        (baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor) / 5,
        // Proporción basada en el vuelo en Y
        (baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor) / 5,
        // Altura mínima establecida de 0.25 metros
        0.25
            };

            // Se toma la altura máxima de las condiciones anteriores y se redondea a múltiplos de 0.05 m
            baseHormigonDimensiones.Altura = Math.Ceiling(condicionesAltura.Max() * 20) / 20;

            // Retornamos el objeto con todas las dimensiones calculadas
            return baseHormigonDimensiones;
        }

        // Método que obtiene los esfuerzos (cargas internas) que actúan sobre la base de hormigón
        public BaseHormigonEsfuerzos ObtenerEsfuerzos(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            // Creamos un objeto donde vamos a almacenar los esfuerzos calculados
            var baseHormigonEsfuerzos = new BaseHormigonEsfuerzos
            {
                // Cálculo del esfuerzo normal (axial) sobre la base
                Normal = baseHormigon.EsfuerzoAxil.Valor +
                         // Peso propio del hormigón
                         baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.Altura * baseHormigon.PesoEspecificoHormigon.Valor +
                         // Peso del suelo que actúa sobre la base
                         baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura) * baseHormigon.PesoEspecificoSuelo.Valor,

                // Cálculo del momento flector en X (Momento = momento de columna + efecto del corte multiplicado por altura)
                MomentoX = baseHormigon.MomentoX.Valor + baseHormigon.CorteX.Valor * baseHormigonDimensiones.Altura,
                // Cálculo del momento flector en Y
                MomentoY = baseHormigon.MomentoY.Valor + baseHormigon.CorteY.Valor * baseHormigonDimensiones.Altura,

                // Cortes directos en X e Y (ya provistos por la columna)
                CorteX = baseHormigon.CorteX.Valor,
                CorteY = baseHormigon.CorteY.Valor
            };

            // Retornamos los esfuerzos calculados
            return baseHormigonEsfuerzos;
        }


        // Método que verifica la seguridad y comportamiento de la base de hormigón
        public BaseHormigonVerificaciones VerificarBase(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonEsfuerzos baseHormigonEsfuerzos)
        {
            // Creamos un objeto donde se van a almacenar todas las verificaciones
            var baseHormigonVerificaciones = new BaseHormigonVerificaciones
            {
                // Cálculo del coeficiente de seguridad frente al vuelco
                // Se toma el mínimo entre los dos ejes
                CoeficienteSeguridadVuelco = Math.Min(
                    (baseHormigonEsfuerzos.Normal * baseHormigonDimensiones.AnchoX) / (2 * baseHormigonEsfuerzos.MomentoX),
                    (baseHormigonEsfuerzos.Normal * baseHormigonDimensiones.AnchoY) / (2 * baseHormigonEsfuerzos.MomentoY)
                )
            };

            // Verificación de que la base cumple con el coeficiente mínimo de vuelco
            baseHormigonVerificaciones.VerificaVuelco = baseHormigonVerificaciones.CoeficienteSeguridadVuelco >= 1.5;

            // Cálculo de la excentricidad de la carga respecto de los ejes X e Y
            baseHormigonVerificaciones.ExcentricidadX = baseHormigonEsfuerzos.MomentoX / baseHormigonEsfuerzos.Normal;
            baseHormigonVerificaciones.ExcentricidadY = baseHormigonEsfuerzos.MomentoY / baseHormigonEsfuerzos.Normal;

            // Cálculo de tensiones máximas y mínimas en X según la excentricidad
            if (baseHormigonVerificaciones.ExcentricidadX == 0)
            {
                // Caso de carga centrada: tensión uniforme
                baseHormigonVerificaciones.TensionMaximaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
                baseHormigonVerificaciones.TensionMinimaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);
            }
            else if (baseHormigonVerificaciones.ExcentricidadX <= baseHormigonDimensiones.AnchoX / 6)
            {
                // Caso de carga excéntrica moderada
                baseHormigonVerificaciones.TensionMaximaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) *
                    (1 + 6 * baseHormigonVerificaciones.ExcentricidadX / baseHormigonDimensiones.AnchoX);
                baseHormigonVerificaciones.TensionMinimaX = baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) *
                    (1 - 6 * baseHormigonVerificaciones.ExcentricidadX / baseHormigonDimensiones.AnchoX);
            }
            else if (baseHormigonVerificaciones.ExcentricidadX > baseHormigonDimensiones.AnchoX / 6)
            {
                // Caso de carga muy excéntrica
                baseHormigonVerificaciones.TensionMaximaX = 4 * baseHormigonEsfuerzos.Normal / (3 * (baseHormigonDimensiones.AnchoX - 2 * baseHormigonVerificaciones.ExcentricidadX) * baseHormigonDimensiones.AnchoY);
                baseHormigonVerificaciones.TensionMinimaX = 0;
            }

            // Lo mismo pero para el eje Y
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

            // Verificación de que las tensiones máximas y medias no superen la carga admisible
            baseHormigonVerificaciones.VerificaTensionAdmisible = baseHormigonVerificaciones.TensionMaximaX <= 1.25 * baseHormigon.CargaAdmisible.Valor &&
                baseHormigonVerificaciones.TensionMaximaY <= 1.25 * baseHormigon.CargaAdmisible.Valor &&
                (baseHormigonVerificaciones.TensionMaximaX + baseHormigonVerificaciones.TensionMinimaX) / 2 <= baseHormigon.CargaAdmisible.Valor &&
                (baseHormigonVerificaciones.TensionMaximaY + baseHormigonVerificaciones.TensionMinimaY) / 2 <= baseHormigon.CargaAdmisible.Valor;

            // Cálculo del asentamiento medio de la base
            baseHormigonVerificaciones.AsentamientoMedio = baseHormigonEsfuerzos.Normal / (baseHormigon.ModuloBalasto.Valor * baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);

            // Cálculo del asentamiento máximo considerando momento flector
            baseHormigonVerificaciones.AsentamientoMaximo = (1 / baseHormigon.ModuloBalasto.Valor) *
                (baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                6 * baseHormigon.MomentoX.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoY) +
                6 * baseHormigon.MomentoY.Valor / (baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoX));

            // Cálculo del asentamiento mínimo
            var asentamiento = (1 / baseHormigon.ModuloBalasto.Valor) *
                (
                    baseHormigonEsfuerzos.Normal / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY)
                    - 6 * baseHormigon.MomentoX.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoY)
                    - 6 * baseHormigon.MomentoY.Valor / (baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoX)
                );

            baseHormigonVerificaciones.AsentamientoMinimo = Math.Max(0, asentamiento);

            // Verificación de que el asentamiento medio no supere el valor máximo permitido (0.035 m)
            baseHormigonVerificaciones.VerificaAsentamientoMedio = baseHormigonVerificaciones.AsentamientoMedio <= 0.035;

            // Cálculo de distorsión angular (asentamiento diferencial)
            baseHormigonVerificaciones.DistorsionAngular = (baseHormigonVerificaciones.AsentamientoMaximo - baseHormigonVerificaciones.AsentamientoMinimo) /
                Math.Sqrt(Math.Pow(baseHormigonDimensiones.AnchoX, 2) + Math.Pow(baseHormigonDimensiones.AnchoY, 2));

            // Verificación de que la distorsión angular esté dentro del límite permisible
            baseHormigonVerificaciones.VerificaAsentamientoDiferencial = baseHormigonVerificaciones.DistorsionAngular <= 0.002;

            return baseHormigonVerificaciones;
        }

        // Método simple que verifica que la tensión promedio de la base no supere la carga admisible
        public bool VerificarTension(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            // Cálculo de la carga total considerando peso propio del hormigón y del suelo
            var cargaTotal = baseHormigon.EsfuerzoAxil.Valor /
                             (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            // Retorna verdadero si la carga total está dentro de la admisible
            return cargaTotal < baseHormigon.CargaAdmisible.Valor;
        }

        // Método que calcula la cuantía de acero necesaria para la base de hormigón
        public BaseHormigonCuantia CalcularCuantia(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var baseHormigonCuantia = new BaseHormigonCuantia
            {
                // Cálculo del esfuerzo axial mayorado considerando cargas permanentes y variables
                EsfuerzoAxilMayorado = new double[]
                {
            1.4 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor,
            1.2 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor +
            1.6 * (baseHormigon.PorcentajeCargaL.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor
                }.Max()
            };

            // Cálculo de la carga mayorada por unidad de área
            baseHormigonCuantia.CargaMayorada = baseHormigonCuantia.EsfuerzoAxilMayorado /
                (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY);

            // Cálculo de momentos mayorados en X e Y
            baseHormigonCuantia.MomentoMayoradoX = baseHormigonCuantia.CargaMayorada *
                Math.Pow(baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor, 2) *
                baseHormigonDimensiones.AnchoY / 8;

            baseHormigonCuantia.MomentoMayoradoY = baseHormigonCuantia.CargaMayorada *
                Math.Pow(baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor, 2) *
                baseHormigonDimensiones.AnchoX / 8;

            // Conversión a momento nominal (factor de reducción)
            baseHormigonCuantia.MomentoNominalX = baseHormigonCuantia.MomentoMayoradoX / 0.9;
            baseHormigonCuantia.MomentoNominalY = baseHormigonCuantia.MomentoMayoradoY / 0.9;

            // Cálculo de factores adimensionales para diseño de acero
            baseHormigonCuantia.FactorAdimensionalX = baseHormigonCuantia.MomentoNominalX /
                (baseHormigonDimensiones.AnchoY * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02), 2) *
                 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000);

            baseHormigonCuantia.FactorAdimensionalY = baseHormigonCuantia.MomentoNominalY /
                (baseHormigonDimensiones.AnchoX * Math.Pow((baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02), 2) *
                 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000);

            // Cálculo de la cuantía mecánica de acero
            baseHormigonCuantia.CuantiaMecanicaX = 1 - Math.Sqrt(1 - 2 * baseHormigonCuantia.FactorAdimensionalX);
            baseHormigonCuantia.CuantiaMecanicaY = 1 - Math.Sqrt(1 - 2 * baseHormigonCuantia.FactorAdimensionalY);

            // Cálculo de la cuantía de cálculo considerando el acero
            baseHormigonCuantia.CuantiaCalculoX = baseHormigonCuantia.CuantiaMecanicaX *
                0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor /
                (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            baseHormigonCuantia.CuantiaCalculoY = baseHormigonCuantia.CuantiaMecanicaY *
                0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor /
                (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            // Cuantía máxima permitida
            baseHormigonCuantia.CuantiaMaxima = 0.85 * 3 * 0.85 * baseHormigon.ResistenciaCaracteristicaHormigon.Valor /
                (8 * (baseHormigon.TensionFluenciaAcero.Valor / 1000));

            // Verificación de que las cuantías calculadas no superen la máxima
            baseHormigonCuantia.VerificaCuantiaMaxima = baseHormigonCuantia.CuantiaCalculoX < baseHormigonCuantia.CuantiaMaxima &&
                                                        baseHormigonCuantia.CuantiaCalculoY < baseHormigonCuantia.CuantiaMaxima;

            // Cuantía mínima según normativa
            baseHormigonCuantia.CuantiaMinima = 1.4 / (baseHormigon.TensionFluenciaAcero.Valor / 1000);

            // Ajuste de la cuantía adoptada dentro de límites mínimos y máximos
            baseHormigonCuantia.CuantiaAdoptadaX = Math.Min(Math.Max(baseHormigonCuantia.CuantiaCalculoX, baseHormigonCuantia.CuantiaMinima), 4 / 3.0 * baseHormigonCuantia.CuantiaCalculoX);
            baseHormigonCuantia.CuantiaAdoptadaY = Math.Min(Math.Max(baseHormigonCuantia.CuantiaCalculoY, baseHormigonCuantia.CuantiaMinima), 4 / 3.0 * baseHormigonCuantia.CuantiaCalculoY);

            // Cálculo del área de acero necesaria en X e Y (cm²)
            baseHormigonCuantia.AreaAceroX = baseHormigonCuantia.CuantiaAdoptadaX * baseHormigonDimensiones.AnchoY *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) * 10000;

            baseHormigonCuantia.AreaAceroY = baseHormigonCuantia.CuantiaAdoptadaY * baseHormigonDimensiones.AnchoX *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) * 10000;

            return baseHormigonCuantia;
        }


        // Método que calcula la armadura de la base de hormigón
        public BaseHormigonArmadura CalcularArmadura(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonCuantia baseHormigonCuantia)
        {
            // Tomamos los diámetros de las barras de acero en X e Y
            var diametrosBarras = (baseHormigon.DiametroBarrasX.Valor, baseHormigon.DiametroBarrasY.Valor);

            // Calculamos el área de cada barra individual
            var areasBarrasIndividuales = (Math.PI * Math.Pow(diametrosBarras.Item1, 2) / 4, Math.PI * Math.Pow(diametrosBarras.Item2, 2) / 4);

            // Calculamos la cantidad de barras necesarias en X e Y
            var cantidadBarras = (
                Math.Ceiling(baseHormigonCuantia.AreaAceroX / (10000 * areasBarrasIndividuales.Item1)),
                Math.Ceiling(baseHormigonCuantia.AreaAceroY / (10000 * areasBarrasIndividuales.Item2))
            );

            // Calculamos la separación entre barras (en cm), respetando recubrimientos y límites máximos
            var separacionBarras = (
                Math.Round(100 * Math.Min((baseHormigonDimensiones.AnchoY - 2 * baseHormigon.RecubrimientoHormigon.Valor) / (cantidadBarras.Item1 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item1, 0.3 }.Min())),

                Math.Round(100 * Math.Min((baseHormigonDimensiones.AnchoX - 2 * baseHormigon.RecubrimientoHormigon.Valor) / (cantidadBarras.Item2 - 1),
                         new double[] { 2.5 * baseHormigonDimensiones.Altura, 25 * diametrosBarras.Item2, 0.3 }.Min()))
            );

            // Creamos el objeto de armadura con las cantidades y separaciones calculadas
            var baseHormigonArmadura = new BaseHormigonArmadura()
            {
                CantidadBarrasX = cantidadBarras.Item1,
                CantidadBarrasY = cantidadBarras.Item2,
                SeparacionBarrasX = separacionBarras.Item1,
                SeparacionBarrasY = separacionBarras.Item2,
            };

            return baseHormigonArmadura;
        }

        // Método que verifica el punzonado de la base de hormigón (resistencia a corte alrededor de la columna)
        public BaseHormigonVerificacionPunzonado VerificarPunzonado(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var verificacionPunzonado = new BaseHormigonVerificacionPunzonado();

            // Calculamos combinaciones de carga mayoradas según cargas permanentes y variables
            var combinacionesCarga = new double[]
            {
        1.4 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor,
        1.2 * (baseHormigon.PorcentajeCargaD.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor +
        1.6 * (baseHormigon.PorcentajeCargaL.Valor / 100) * baseHormigon.EsfuerzoAxil.Valor
            };

            // Tomamos la mayor de las combinaciones como esfuerzo axial mayorado
            verificacionPunzonado.EsfuerzoAxilMayorado = combinacionesCarga.Max();

            // Cálculo de la carga total sobre la base (peso propio + suelo)
            verificacionPunzonado.CargaTotal = baseHormigon.EsfuerzoAxil.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                                               baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                                               baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura);

            // Resistencia requerida al punzonado
            verificacionPunzonado.ResistenciaRequerida = verificacionPunzonado.EsfuerzoAxilMayorado - verificacionPunzonado.CargaTotal *
                (baseHormigon.AnchoColumnaX.Valor + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02)) *
                (baseHormigon.AnchoColumnaY.Valor + (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02));

            // Perímetro crítico para el punzonado (B0)
            verificacionPunzonado.B0 = 2 * (baseHormigon.AnchoColumnaX.Valor + baseHormigon.AnchoColumnaY.Valor) +
                                       4 * (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02);

            // Relación entre los lados de la columna
            verificacionPunzonado.B = new double[] { baseHormigon.AnchoColumnaX.Valor, baseHormigon.AnchoColumnaY.Valor }.Max() /
                                      new double[] { baseHormigon.AnchoColumnaX.Valor, baseHormigon.AnchoColumnaY.Valor }.Min();

            // Calculamos las resistencias nominales según distintas fórmulas de diseño
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

            // Tomamos la mínima resistencia nominal para el diseño
            verificacionPunzonado.ResistenciaNominal = resistenciasNominales.Min();

            // Resistencia de diseño considerando factor de seguridad
            verificacionPunzonado.ResistenciaDiseno = verificacionPunzonado.ResistenciaNominal * 0.75;

            // Verificamos si la base cumple con la resistencia al punzonado
            verificacionPunzonado.CumpleVerificacion = verificacionPunzonado.ResistenciaRequerida <= verificacionPunzonado.ResistenciaDiseno;

            return verificacionPunzonado;
        }

        // Método que verifica la resistencia al corte de la base de hormigón en X e Y
        public BaseHormigonVerificacionCorte VerificarCorte(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones)
        {
            var verificacionCorte = new BaseHormigonVerificacionCorte
            {
                // Cálculo de la carga total sobre la base (peso propio + suelo)
                CargaTotal = baseHormigon.EsfuerzoAxil.Valor / (baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY) +
                             baseHormigon.PesoEspecificoHormigon.Valor * baseHormigonDimensiones.Altura +
                             baseHormigon.PesoEspecificoSuelo.Valor * (baseHormigon.NivelFundacion.Valor - baseHormigonDimensiones.Altura)
            };

            // Resistencia requerida al corte en dirección X
            verificacionCorte.ResistenciaRequeridaX = verificacionCorte.CargaTotal *
                (((baseHormigonDimensiones.AnchoX - baseHormigon.AnchoColumnaX.Valor) / 2 -
                  (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02))) * baseHormigonDimensiones.AnchoY;

            // Resistencia requerida al corte en dirección Y
            verificacionCorte.ResistenciaRequeridaY = verificacionCorte.CargaTotal *
                (((baseHormigonDimensiones.AnchoY - baseHormigon.AnchoColumnaY.Valor) / 2 -
                  (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02))) * baseHormigonDimensiones.AnchoX;

            // Resistencia nominal al corte según características del hormigón
            verificacionCorte.ResistenciaNominalX = baseHormigonDimensiones.AnchoY *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
                Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6;

            verificacionCorte.ResistenciaNominalY = baseHormigonDimensiones.AnchoX *
                (baseHormigonDimensiones.Altura - baseHormigon.RecubrimientoHormigon.Valor - 0.02) *
                Math.Sqrt(baseHormigon.ResistenciaCaracteristicaHormigon.Valor * 1000) / 6;

            // Resistencia de diseño considerando factor de seguridad
            verificacionCorte.ResistenciaDisenoX = verificacionCorte.ResistenciaNominalX * 0.75;
            verificacionCorte.ResistenciaDisenoY = verificacionCorte.ResistenciaNominalY * 0.75;

            // Verificación de que la resistencia requerida no supere la resistencia de diseño
            verificacionCorte.CumpleVerificacion = verificacionCorte.ResistenciaRequeridaX <= verificacionCorte.ResistenciaDisenoX &&
                                                   verificacionCorte.ResistenciaRequeridaY <= verificacionCorte.ResistenciaDisenoY;

            return verificacionCorte;
        }

        // Método que realiza el cómputo de materiales y costos de la base de hormigón
        public BaseHormigonComputo Computo(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonArmadura baseHormigonArmadura)
        {
            // Volumen total de hormigón de la base
            var volumenHormigon = baseHormigonDimensiones.AnchoX * baseHormigonDimensiones.AnchoY * baseHormigonDimensiones.Altura;

            // Longitud total de barras de refuerzo en X e Y (considerando recubrimiento y solapes)
            var longitudBarrasX = baseHormigonArmadura.CantidadBarrasX * (baseHormigonDimensiones.AnchoX + 0.5);
            var longitudBarrasY = baseHormigonArmadura.CantidadBarrasY * (baseHormigonDimensiones.AnchoY + 0.5);

            // Diámetros de barras de acero
            var diametrosBarras = (baseHormigon.DiametroBarrasX.Valor, baseHormigon.DiametroBarrasY.Valor);

            // Área de cada barra individual
            var areasBarrasIndividuales = (Math.PI * Math.Pow(diametrosBarras.Item1, 2) / 4, Math.PI * Math.Pow(diametrosBarras.Item2, 2) / 4);

            // Peso de las barras de refuerzo en kg (densidad acero = 7850 kg/m³)
            var pesoBarrasX = longitudBarrasX * areasBarrasIndividuales.Item1 * 7850;
            var pesoBarrasY = longitudBarrasY * areasBarrasIndividuales.Item2 * 7850;

            // Volumen de excavación considerando coeficiente de esponjamiento del suelo
            var volumenExcavacion = volumenHormigon * baseHormigon.CoeficienteEsponjamiento.Valor;

            // Cálculo de costos
            var costoHormigon = volumenHormigon * baseHormigon.CostoM3Hormigon.Valor;
            var costoAcero = (pesoBarrasX + pesoBarrasY) * baseHormigon.CostoKgAcero.Valor;
            var costoExcavacion = volumenExcavacion * baseHormigon.CostoM3Excavacion.Valor;

            // Creamos el objeto de cómputo con resultados finales
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