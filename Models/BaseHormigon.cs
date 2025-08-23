namespace CalculoBasesAIE.Models
{
    public class BaseHormigon
    {
        public long Id { get; set; }
        public string Nombre { get; set; }

        // Parámetros básicos
        public ValueUnitPair EsfuerzoAxil { get; set; }
        public ValueUnitPair EsfuerzoCorteX { get; set; }
        public ValueUnitPair EsfuerzoCorteY { get; set; }
        public ValueUnitPair MomentoX { get; set; }
        public ValueUnitPair MomentoY { get; set; }
        public ValueUnitPair CargaAdmisible { get; set; }
        public ValueUnitPair ModuloBalastoVertical { get; set; }

        // Ajustes avanzados
        public ValueUnitPair PorcentajeCargaD { get; set; }
        public ValueUnitPair PorcentajeCargaL { get; set; }
        public ValueUnitPair AnchoColumnaX { get; set; }
        public ValueUnitPair AnchoColumnaY { get; set; }
        public ValueUnitPair PesoEspecificoSuelo { get; set; }
        public ValueUnitPair NivelFundacion { get; set; }
        public ValueUnitPair PesoEspecificoHormigon { get; set; }
        public ValueUnitPair ResistenciaCaracteristicaHormigon { get; set; }
        public ValueUnitPair RecubrimientoHormigon { get; set; }
        public ValueUnitPair TensionFluenciaAcero { get; set; }
        public ValueUnitPair DiametroBarrasX { get; set; }
        public ValueUnitPair DiametroBarrasY { get; set; }
    }
}
