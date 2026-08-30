using WebApi.Models;

namespace WebApi.Interface
{
    public interface ITipoIndicadorSaludService
    {
        Task<IEnumerable<TipoIndicadorSalud>> GetAllAsync();
        Task<TipoIndicadorSalud?> GetByIdAsync(int id);
        Task<TipoIndicadorSalud> CreateAsync(TipoIndicadorSalud entity);
        Task<bool> UpdateAsync(int id, TipoIndicadorSalud entity);
        Task<bool> DeleteAsync(int id);
    }
}
