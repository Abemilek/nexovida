using WebApi.Models;

namespace WebApi.Interface
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<Usuario> CreateAsync(Usuario entity);
        Task<bool> UpdateAsync(int id, Usuario entity);
        Task<bool> DeleteAsync(int id);
        bool VerifyPassword(string plainTextPassword, byte[] hash, byte[] salt);
        Task<bool> SetTwoFactorAsync(int id, bool enabled, string? secret);
        Task UpdateUltimoAccesoAsync(int id);
    }
}
