using WebApi.Models;

namespace WebApi.Interface
{
    public interface IPacienteEnfermedadService
    {
        Task<IEnumerable<PacienteEnfermedad>> GetAllAsync();
        Task<PacienteEnfermedad?> GetByIdAsync(int id);
        Task<PacienteEnfermedad> CreateAsync(PacienteEnfermedad entity);
        Task<bool> UpdateAsync(int id, PacienteEnfermedad entity);
        Task<bool> DeleteAsync(int id);
    }
}
