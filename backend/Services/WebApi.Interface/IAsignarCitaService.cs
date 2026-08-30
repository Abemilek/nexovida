using WebApi.Models;

namespace WebApi.Interface
{
    public interface IAsignarCitaService
    {
        Task<IEnumerable<AsignarCita>> GetAllAsync();
        Task<AsignarCita?> GetByIdAsync(int id);
        Task<AsignarCita> CreateAsync(AsignarCita entity);
        Task<bool> UpdateAsync(int id, AsignarCita entity);
        Task<bool> DeleteAsync(int id);
    }
}
