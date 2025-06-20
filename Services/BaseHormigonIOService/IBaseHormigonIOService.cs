using CalculoBasesAIE.Models;

namespace CalculoBasesAIE.Services.BaseHormigonIOService
{
    public interface IBaseHormigonIOService
    {
        Task<byte[]?> GenerateExcelAsync(long baseId);
        Task<byte[]?> GenerateCsvAsync(long baseId);
        Task<byte[]?> GeneratePdfAsync(long baseId);
        Task<BaseHormigon?> ImportBaseHormigonAsync(IFormFile file);
        BaseHormigon ParseBaseHormigonFromCsv(Stream stream);
        BaseHormigon ParseBaseHormigonFromExcel(Stream stream);
    }
}