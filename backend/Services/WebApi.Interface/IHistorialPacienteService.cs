using WebApi.Models;

namespace WebApi.Interface
{
    public interface IHistorialPacienteService
    {
        Task<IEnumerable<HistorialPaciente>> GetAllAsync();
        Task<HistorialPaciente?> GetByIdAsync(long id);
        Task<HistorialPaciente> CreateAsync(HistorialPaciente entity);
        Task<bool> UpdateAsync(long id, HistorialPaciente entity);
        Task<bool> DeleteAsync(long id);
    }
}
