using CalculoBasesAIE.Models;

namespace CalculoBasesAIE.Services
{
    public interface IExcelService
    {
        BaseHormigon ParseBaseHormigonFromCsv(Stream stream);
        BaseHormigon ParseBaseHormigonFromExcel(Stream stream);
    }
}