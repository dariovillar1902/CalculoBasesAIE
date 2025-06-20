using CalculoBasesAIE.Models;

namespace CalculoBasesAIE.Services.BaseHormigonService
{
    public interface IBaseHormigonService
    {
        Task<List<BaseHormigon>> GetAllBasesAsync();
        Task<BaseHormigon?> GetBaseByIdAsync(long id);
        Task<BaseHormigonDimensiones?> GetDimensionesAsync(long id);
        Task<bool?> VerificarTensionAdmisibleAsync(long id);
        Task<BaseHormigonCuantia?> CalcularCuantiaAsync(long id);
        Task<BaseHormigonArmadura?> CalcularArmaduraAsync(long id);
        Task<BaseHormigonVerificacionPunzonado?> VerificarPunzonadoAsync(long id);
        Task<BaseHormigonVerificacionCorte?> VerificarCorteAsync(long id);
        Task<BaseHormigon?> CreateAsync(BaseHormigon baseHormigon);
        Task<bool> UpdateAsync(long id, BaseHormigon baseHormigon);
        Task<bool> DeleteAsync(long id);
        Task<BaseHormigonArmadura?> CalcularArmaduraConDiametrosAsync(long id, BaseHormigonDiametrosBarras nuevosDiametros);
        void ConvertirUnidades(BaseHormigon baseHormigon);
        BaseHormigonDimensiones EstimarDimensiones(BaseHormigon baseHormigon);
        bool VerificarTension(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones);
        BaseHormigonCuantia CalcularCuantia(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones);
        BaseHormigonArmadura CalcularArmadura(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones, BaseHormigonCuantia baseHormigonCuantia);
        BaseHormigonVerificacionPunzonado VerificarPunzonado(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones);
        BaseHormigonVerificacionCorte VerificarCorte(BaseHormigon baseHormigon, BaseHormigonDimensiones baseHormigonDimensiones);
    }
}