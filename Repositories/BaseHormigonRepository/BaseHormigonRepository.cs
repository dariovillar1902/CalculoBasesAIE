using CalculoBasesAIE.Models;
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
            var existingEntity = await context.BasesHormigon.FindAsync(baseHormigon.Id);
            if (existingEntity is null) return;

            existingEntity.Nombre = baseHormigon.Nombre;

            existingEntity.EsfuerzoAxil.Valor = baseHormigon.EsfuerzoAxil.Valor;
            existingEntity.EsfuerzoAxil.Unidad = baseHormigon.EsfuerzoAxil.Unidad;
            existingEntity.EsfuerzoAxil.Tipo = baseHormigon.EsfuerzoAxil.Tipo;

            existingEntity.PorcentajeCargaD.Valor = baseHormigon.PorcentajeCargaD.Valor;
            existingEntity.PorcentajeCargaD.Unidad = baseHormigon.PorcentajeCargaD.Unidad;
            existingEntity.PorcentajeCargaD.Tipo = baseHormigon.PorcentajeCargaD.Tipo;

            existingEntity.PorcentajeCargaL.Valor = baseHormigon.PorcentajeCargaL.Valor;
            existingEntity.PorcentajeCargaL.Unidad = baseHormigon.PorcentajeCargaL.Unidad;
            existingEntity.PorcentajeCargaL.Tipo = baseHormigon.PorcentajeCargaL.Tipo;

            existingEntity.AnchoColumnaX.Valor = baseHormigon.AnchoColumnaX.Valor;
            existingEntity.AnchoColumnaX.Unidad = baseHormigon.AnchoColumnaX.Unidad;
            existingEntity.AnchoColumnaX.Tipo = baseHormigon.AnchoColumnaX.Tipo;

            existingEntity.AnchoColumnaY.Valor = baseHormigon.AnchoColumnaY.Valor;
            existingEntity.AnchoColumnaY.Unidad = baseHormigon.AnchoColumnaY.Unidad;
            existingEntity.AnchoColumnaY.Tipo = baseHormigon.AnchoColumnaY.Tipo;

            existingEntity.CargaAdmisible.Valor = baseHormigon.CargaAdmisible.Valor;
            existingEntity.CargaAdmisible.Unidad = baseHormigon.CargaAdmisible.Unidad;
            existingEntity.CargaAdmisible.Tipo = baseHormigon.CargaAdmisible.Tipo;

            existingEntity.PesoEspecificoSuelo.Valor = baseHormigon.PesoEspecificoSuelo.Valor;
            existingEntity.PesoEspecificoSuelo.Unidad = baseHormigon.PesoEspecificoSuelo.Unidad;
            existingEntity.PesoEspecificoSuelo.Tipo = baseHormigon.PesoEspecificoSuelo.Tipo;

            existingEntity.NivelFundacion.Valor = baseHormigon.NivelFundacion.Valor;
            existingEntity.NivelFundacion.Unidad = baseHormigon.NivelFundacion.Unidad;
            existingEntity.NivelFundacion.Tipo = baseHormigon.NivelFundacion.Tipo;

            existingEntity.PesoEspecificoHormigon.Valor = baseHormigon.PesoEspecificoHormigon.Valor;
            existingEntity.PesoEspecificoHormigon.Unidad = baseHormigon.PesoEspecificoHormigon.Unidad;
            existingEntity.PesoEspecificoHormigon.Tipo = baseHormigon.PesoEspecificoHormigon.Tipo;

            existingEntity.ResistenciaCaracteristicaHormigon.Valor = baseHormigon.ResistenciaCaracteristicaHormigon.Valor;
            existingEntity.ResistenciaCaracteristicaHormigon.Unidad = baseHormigon.ResistenciaCaracteristicaHormigon.Unidad;
            existingEntity.ResistenciaCaracteristicaHormigon.Tipo = baseHormigon.ResistenciaCaracteristicaHormigon.Tipo;

            existingEntity.RecubrimientoHormigon.Valor = baseHormigon.RecubrimientoHormigon.Valor;
            existingEntity.RecubrimientoHormigon.Unidad = baseHormigon.RecubrimientoHormigon.Unidad;
            existingEntity.RecubrimientoHormigon.Tipo = baseHormigon.RecubrimientoHormigon.Tipo;

            existingEntity.TensionFluenciaAcero.Valor = baseHormigon.TensionFluenciaAcero.Valor;
            existingEntity.TensionFluenciaAcero.Unidad = baseHormigon.TensionFluenciaAcero.Unidad;
            existingEntity.TensionFluenciaAcero.Tipo = baseHormigon.TensionFluenciaAcero.Tipo;

            existingEntity.DiametroBarrasX.Valor = baseHormigon.DiametroBarrasX.Valor;
            existingEntity.DiametroBarrasX.Unidad = baseHormigon.DiametroBarrasX.Unidad;
            existingEntity.DiametroBarrasX.Tipo = baseHormigon.DiametroBarrasX.Tipo;

            existingEntity.DiametroBarrasY.Valor = baseHormigon.DiametroBarrasY.Valor;
            existingEntity.DiametroBarrasY.Unidad = baseHormigon.DiametroBarrasY.Unidad;
            existingEntity.DiametroBarrasY.Tipo = baseHormigon.DiametroBarrasY.Tipo;

            existingEntity.CostoM3Hormigon.Valor = baseHormigon.CostoM3Hormigon.Valor;
            existingEntity.CostoM3Hormigon.Unidad = baseHormigon.CostoM3Hormigon.Unidad;
            existingEntity.CostoM3Hormigon.Tipo = baseHormigon.CostoM3Hormigon.Tipo;

            existingEntity.CostoM3Excavacion.Valor = baseHormigon.CostoM3Excavacion.Valor;
            existingEntity.CostoM3Excavacion.Unidad = baseHormigon.CostoM3Excavacion.Unidad;
            existingEntity.CostoM3Excavacion.Tipo = baseHormigon.CostoM3Excavacion.Tipo;

            existingEntity.CostoKgAcero.Valor = baseHormigon.CostoKgAcero.Valor;
            existingEntity.CostoKgAcero.Unidad = baseHormigon.CostoKgAcero.Unidad;
            existingEntity.CostoKgAcero.Tipo = baseHormigon.CostoKgAcero.Tipo;

            existingEntity.CoeficienteEsponjamiento.Valor = baseHormigon.CoeficienteEsponjamiento.Valor;
            existingEntity.CoeficienteEsponjamiento.Unidad = baseHormigon.CoeficienteEsponjamiento.Unidad;
            existingEntity.CoeficienteEsponjamiento.Tipo = baseHormigon.CoeficienteEsponjamiento.Tipo;

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
                e.RecubrimientoHormigon.Valor == baseHormigon.RecubrimientoHormigon.Valor &&
                e.CorteX.Valor == baseHormigon.CorteX.Valor && 
                e.CorteY.Valor == baseHormigon.CorteY.Valor &&
                e.MomentoX.Valor == baseHormigon.MomentoX.Valor &&
                e.MomentoY.Valor == baseHormigon.MomentoY.Valor &&
                e.ModuloBalasto.Valor == baseHormigon.ModuloBalasto.Valor &&
                e.CostoKgAcero.Valor == baseHormigon.CostoKgAcero.Valor &&
                e.CostoM3Excavacion.Valor == baseHormigon.CostoM3Excavacion.Valor &&
                e.CostoM3Hormigon.Valor == baseHormigon.CostoM3Hormigon.Valor &&
                e.CoeficienteEsponjamiento.Valor == baseHormigon.CoeficienteEsponjamiento.Valor
            );
        }
    }
}