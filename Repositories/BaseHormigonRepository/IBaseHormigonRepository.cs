using CalculoBasesAIE.Models;

namespace CalculoBasesAIE.Repositories.BaseHormigonRepository
{
    public interface IBaseHormigonRepository
    {
        Task<List<BaseHormigon>> GetAllAsync();
        Task<BaseHormigon?> GetByIdAsync(long id);
        Task AddAsync(BaseHormigon baseHormigon);
        Task UpdateAsync(BaseHormigon baseHormigon);
        Task DeleteAsync(BaseHormigon baseHormigon);
        Task<bool> ExistsAsync(long id);
        Task<BaseHormigon?> GetDuplicateAsync(BaseHormigon baseHormigon);
    }
}