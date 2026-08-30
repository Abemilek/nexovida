using WebApi.Models;

namespace WebApi.Interface
{
    public interface IIndicadorSaludService
    {
        Task<IEnumerable<IndicadorSalud>> GetAllAsync();
        Task<IndicadorSalud?> GetByIdAsync(long id);
        Task<IndicadorSalud> CreateAsync(IndicadorSalud entity);
        Task<bool> UpdateAsync(long id, IndicadorSalud entity);
        Task<bool> DeleteAsync(long id);
    }
}
