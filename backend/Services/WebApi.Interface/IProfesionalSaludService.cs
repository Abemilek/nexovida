using WebApi.Models;

namespace WebApi.Interface
{
    public interface IProfesionalSaludService
    {
        Task<IEnumerable<ProfesionalSalud>> GetAllAsync();
        Task<ProfesionalSalud?> GetByIdAsync(int id);
        Task<ProfesionalSalud> CreateAsync(ProfesionalSalud entity);
        Task<bool> UpdateAsync(int id, ProfesionalSalud entity);
        Task<bool> DeleteAsync(int id);
    }
}
