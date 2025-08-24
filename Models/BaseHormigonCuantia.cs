namespace CalculoBasesAIE.Models
{
    public class BaseHormigonCuantia
    {
        public long Id { get; set; }

        // Efforts and loads
        public double EsfuerzoAxilMayorado { get; set; }   // kN
        public double CargaMayorada1 { get; set; }         // kN/m²
        public double CargaMayorada2 { get; set; }         // kN/m²
        public double CargaMayorada { get; set; }          // kN/m²

        // Moments
        public double MomentoMayorado { get; set; }        // kN·m
        public double MomentoMayoradoX { get; set; }       // kN·m
        public double MomentoMayoradoY { get; set; }       // kN·m
        public double MomentoNominalX { get; set; }        // kN·m
        public double MomentoNominalY { get; set; }        // kN·m

        // Derived values
        public double ExcentricidadMayorada { get; set; }  // m
        public double FactorAdimensionalX { get; set; }    // adimensional
        public double FactorAdimensionalY { get; set; }    // adimensional

        // Reinforcement ratios
        public double CuantiaMecanicaX { get; set; }       // adimensional
        public double CuantiaMecanicaY { get; set; }       // adimensional
        public double CuantiaCalculoX { get; set; }        // adimensional
        public double CuantiaCalculoY { get; set; }        // adimensional
        public double CuantiaMaxima { get; set; }          // adimensional
        public bool VerificaCuantiaMaxima { get; set; }
        public double CuantiaMinima { get; set; }          // adimensional
        public double CuantiaAdoptadaX { get; set; }       // adimensional
        public double CuantiaAdoptadaY { get; set; }       // adimensional

        // Steel areas
        public double AreaAceroX { get; set; }             // cm²
        public double AreaAceroY { get; set; }             // cm²
    }
}
