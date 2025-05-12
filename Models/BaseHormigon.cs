namespace CalculoBasesAIE.Models
{
    public class BaseHormigon
    {
        public long Id { get; set; }
        public long EsfuerzoAxil { get; set; }
        public float PorcentajeCargaD {  get; set; }
        public float PorcentajeCargaL {  get; set; }
        public double AnchoColumnaX {  get; set; }
        public double AnchoColumnaY { get; set; }
        public long CargaAdmisible { get; set; }
        public long PesoEspecificoSuelo { get; set; }
        public double NivelFundacion { get; set; }
        public long FactorSeguridad { get; set; }
        public long PesoEspecificoHormigon { get; set; }
        public long ResistenciaCaracteristicaHormigon { get; set; }
        public double RecubrimientoHormigon { get; set; }
        public long TensionFluenciaAcero { get; set; }
    }
}
