using WebApi.Models;

namespace WebApi.Interface
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int id);
        Task<Rol> CreateAsync(Rol entity);
        Task<bool> UpdateAsync(int id, Rol entity);
        Task<bool> DeleteAsync(int id);
    }
}
