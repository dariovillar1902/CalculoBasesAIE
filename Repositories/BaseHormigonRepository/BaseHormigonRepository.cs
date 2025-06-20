using CalculoBasesAIE.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace CalculoBasesAIE.Repositories.BaseHormigonRepository
{
    public class BaseHormigonRepository(BaseHormigonContext context) : IBaseHormigonRepository
    {
        public Task<List<BaseHormigon>> GetAllAsync() =>
            context.BasesHormigon.ToListAsync();

        public Task<BaseHormigon?> GetByIdAsync(long id) =>
            context.BasesHormigon.FindAsync(id).AsTask();

        public async Task AddAsync(BaseHormigon baseHormigon)
        {
            context.BasesHormigon.Add(baseHormigon);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BaseHormigon baseHormigon)
        {
            context.Entry(baseHormigon).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BaseHormigon baseHormigon)
        {
            context.BasesHormigon.Remove(baseHormigon);
            await context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(long id) =>
            context.BasesHormigon.AnyAsync(e => e.Id == id);

        public async Task<BaseHormigon?> GetDuplicateAsync(BaseHormigon baseHormigon)
        {
            return await context.BasesHormigon.FirstOrDefaultAsync(e =>
                e.EsfuerzoAxil.Valor == baseHormigon.EsfuerzoAxil.Valor &&
                e.PorcentajeCargaD.Valor == baseHormigon.PorcentajeCargaD.Valor &&
                e.PorcentajeCargaL.Valor == baseHormigon.PorcentajeCargaL.Valor &&
                e.CargaAdmisible.Valor == baseHormigon.CargaAdmisible.Valor &&
                e.AnchoColumnaX.Valor == baseHormigon.AnchoColumnaX.Valor &&
                e.AnchoColumnaY.Valor == baseHormigon.AnchoColumnaY.Valor &&
                e.PesoEspecificoSuelo.Valor == baseHormigon.PesoEspecificoSuelo.Valor &&
                e.NivelFundacion.Valor == baseHormigon.NivelFundacion.Valor &&
                e.PesoEspecificoHormigon.Valor == baseHormigon.PesoEspecificoHormigon.Valor &&
                e.ResistenciaCaracteristicaHormigon.Valor == baseHormigon.ResistenciaCaracteristicaHormigon.Valor &&
                e.TensionFluenciaAcero.Valor == baseHormigon.TensionFluenciaAcero.Valor &&
                e.DiametroBarrasX.Valor == baseHormigon.DiametroBarrasX.Valor &&
                e.DiametroBarrasY.Valor == baseHormigon.DiametroBarrasY.Valor &&
                e.RecubrimientoHormigon.Valor == baseHormigon.RecubrimientoHormigon.Valor
            );
        }
    }
}