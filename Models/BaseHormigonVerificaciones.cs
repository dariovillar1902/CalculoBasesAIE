namespace CalculoBasesAIE.Models
{
    public class BaseHormigonVerificaciones
    {
        public long Id { get; set; }
        public double CoeficienteSeguridadVuelco { get; set; }
        public bool VerificaVuelco { get; set; }
        public double CoeficienteSeguridadDeslizamiento { get; set; }
        public bool VerificaDeslizamiento { get; set; }
        public double ExcentricidadX { get; set; }
        public double ExcentricidadY { get; set; }
        public double TensionMaximaX { get; set; }
        public double TensionMinimaX { get; set; }
        public double TensionMaximaY { get; set; }
        public double TensionMinimaY { get; set; }
        public bool VerificaTensionAdmisible { get; set; }
        public double AsentamientoMedio { get; set; }
        public double AsentamientoMaximo { get; set; }
        public double AsentamientoMinimo { get; set; }
        public double DistorsionAngular { get; set; }
        public bool VerificaAsentamientoMedio { get; set; }
        public bool VerificaAsentamientoDiferencial { get; set; }
    }
}
