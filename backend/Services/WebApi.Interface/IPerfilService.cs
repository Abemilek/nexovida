using WebApi.Models;

namespace WebApi.Interface
{
    public interface IPerfilService
    {
        Task<IEnumerable<Perfil>> GetAllAsync();
        Task<Perfil?> GetByIdAsync(int id);
        Task<Perfil> CreateAsync(Perfil entity);
        Task<bool> UpdateAsync(int id, Perfil entity);
        Task<bool> DeleteAsync(int id);
    }
}
