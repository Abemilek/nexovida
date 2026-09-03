using WebApi.Models;

namespace WebApi.Interface
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAllAsync();
        Task<Paciente?> GetByIdAsync(int id);
        Task<Paciente> CreateAsync(Paciente entity);
        Task<bool> UpdateAsync(int id, Paciente entity);
        Task<bool> DeleteAsync(int id);
    }
}
