namespace CalculoBasesAIE.Models
{
    public class BaseHormigonVerificacionCorte
    {
        public long Id { get; set; }
        public double CargaTotal { get; set; }
        public double ResistenciaRequeridaX { get; set; }
        public double ResistenciaRequeridaY { get; set; }
        public double ResistenciaNominalX { get; set; }
        public double ResistenciaNominalY { get; set; }
        public double ResistenciaDisenoX { get; set; }
        public double ResistenciaDisenoY { get; set; }
        public bool CumpleVerificacion { get; set; }
    }
}
