using WebApi.Models;

namespace WebApi.Interface
{
    public interface ICitaService
    {
        Task<IEnumerable<Cita>> GetAllAsync();
        Task<Cita?> GetByIdAsync(int id);
        Task<Cita> CreateAsync(Cita entity);
        Task<bool> UpdateAsync(int id, Cita entity);
        Task<bool> DeleteAsync(int id);
    }
}
