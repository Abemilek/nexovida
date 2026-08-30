using WebApi.Models;

namespace WebApi.Interface
{
    public interface IAlertaService
    {
        Task<IEnumerable<Alerta>> GetAllAsync();
        Task<Alerta?> GetByIdAsync(long id);
        Task<Alerta> CreateAsync(Alerta entity);
        Task<bool> UpdateAsync(long id, Alerta entity);
        Task<bool> DeleteAsync(long id);
    }
}
