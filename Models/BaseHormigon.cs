namespace CalculoBasesAIE.Models
{
    public class BaseHormigon
    {
        public long Id { get; set; }
        public ValueUnitPair EsfuerzoAxil { get; set; }
        public ValueUnitPair PorcentajeCargaD { get; set; }
        public ValueUnitPair PorcentajeCargaL { get; set; }
        public ValueUnitPair AnchoColumnaX { get; set; }
        public ValueUnitPair AnchoColumnaY { get; set; }
        public ValueUnitPair CargaAdmisible { get; set; }
        public ValueUnitPair PesoEspecificoSuelo { get; set; }
        public ValueUnitPair NivelFundacion { get; set; }
        public ValueUnitPair PesoEspecificoHormigon { get; set; }
        public ValueUnitPair ResistenciaCaracteristicaHormigon { get; set; }
        public ValueUnitPair RecubrimientoHormigon { get; set; }
        public ValueUnitPair TensionFluenciaAcero { get; set; }
        public ValueUnitPair DiametroBarrasX { get; set; }
        public ValueUnitPair DiametroBarrasY {  get; set; }
    }
}
