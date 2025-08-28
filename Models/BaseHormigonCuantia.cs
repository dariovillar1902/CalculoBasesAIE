namespace CalculoBasesAIE.Models
{
    public class BaseHormigonCuantia
    {
        public long Id { get; set; }
        public double EsfuerzoAxilMayorado { get; set; }
        public double CargaMayorada { get; set; }
        public double MomentoMayoradoX { get; set; }
        public double MomentoMayoradoY { get; set; }
        public double MomentoNominalX { get; set; }
        public double MomentoNominalY { get; set; }
        public double FactorAdimensionalX { get; set; }
        public double FactorAdimensionalY { get; set; }
        public double CuantiaMecanicaX { get; set; }
        public double CuantiaMecanicaY { get; set; }
        public double CuantiaCalculoX { get; set; }
        public double CuantiaCalculoY { get; set; }
        public double CuantiaMaxima { get; set; }
        public bool VerificaCuantiaMaxima { get; set; }
        public double CuantiaMinima { get; set; }
        public double CuantiaAdoptadaX { get; set; }
        public double CuantiaAdoptadaY { get; set; }
        public double AreaAceroX { get; set; }
        public double AreaAceroY { get; set; }
    }
}
