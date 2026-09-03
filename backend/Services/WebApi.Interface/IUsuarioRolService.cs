using WebApi.Models;

namespace WebApi.Interface
{
    public interface IUsuarioRolService
    {
        Task<IEnumerable<UsuarioRol>> GetAllAsync();
        Task<UsuarioRol?> GetByIdAsync(int id);
        Task<UsuarioRol> CreateAsync(UsuarioRol entity);
        Task<bool> UpdateAsync(int id, UsuarioRol entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<string>> GetRoleNamesForUserAsync(int idUsuario);
    }
}
