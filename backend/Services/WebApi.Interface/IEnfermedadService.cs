using WebApi.Models;

namespace WebApi.Interface
{
    public interface IEnfermedadService
    {
        Task<IEnumerable<Enfermedad>> GetAllAsync();
        Task<Enfermedad?> GetByIdAsync(int id);
        Task<Enfermedad> CreateAsync(Enfermedad entity);
        Task<bool> UpdateAsync(int id, Enfermedad entity);
        Task<bool> DeleteAsync(int id);
    }
}
