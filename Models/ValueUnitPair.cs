using Microsoft.EntityFrameworkCore;

namespace CalculoBasesAIE.Models
{
    [Owned]
    public class ValueUnitPair
    {
        public double Valor { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class ValueUnitPairCsv
    {
        public string Variable { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
    }
}