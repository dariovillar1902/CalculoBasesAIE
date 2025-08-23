namespace CalculoBasesAIE.Models
{
    public class BaseHormigonDimensiones
    {
        public long Id { get; set; }

        // Dimensiones finales
        public double Area { get; set; }
        public double AnchoX { get; set; }
        public double AnchoY { get; set; }
        public double VueloX { get; set; }
        public double VueloY { get; set; }
        public bool VerificaVuelos { get; set; }
        public double Altura { get; set; }

        // Nuevas variables del cálculo
        public double CargaDiseno { get; set; }        // Pd
        public double TensionPromedio { get; set; }    // qAvg
        public double RelacionLados { get; set; }      // AnchoX / AnchoY
        public double AreaNecesaria { get; set; }      // Área antes del ajuste
    }
}
