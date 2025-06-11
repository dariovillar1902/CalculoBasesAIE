namespace CalculoBasesAIE.Models
{
    public class BaseHormigonVerificacionPunzonado
    {
        public long Id { get; set; }
        public double EsfuerzoAxilMayorado { get; set; }
        public double CargaTotal { get; set; }
        public double ResistenciaRequerida { get; set; }
        public double B0 { get; set; }
        public double B { get; set; }
        public double ResistenciaNominal { get; set; }
        public double ResistenciaDiseno { get; set; }
        public bool CumpleVerificacion { get; set; }
    }
}
