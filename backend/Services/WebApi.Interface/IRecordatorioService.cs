using WebApi.Models;

namespace WebApi.Interface
{
    public interface IRecordatorioService
    {
        Task<IEnumerable<Recordatorio>> GetAllAsync();
        Task<Recordatorio?> GetByIdAsync(int id);
        Task<Recordatorio> CreateAsync(Recordatorio entity);
        Task<bool> UpdateAsync(int id, Recordatorio entity);
        Task<bool> DeleteAsync(int id);
    }
}
