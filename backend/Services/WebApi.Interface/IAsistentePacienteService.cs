using WebApi.Models;

namespace WebApi.Interface
{
    public interface IAsistentePacienteService
    {
        Task<IEnumerable<AsistentePaciente>> GetAllAsync();
        Task<AsistentePaciente?> GetByIdAsync(int id);
        Task<AsistentePaciente> CreateAsync(AsistentePaciente entity);
        Task<bool> UpdateAsync(int id, AsistentePaciente entity);
        Task<bool> DeleteAsync(int id);
    }
}
