using WebApi.Models;

namespace WebApi.Interface
{
    public interface ITratamientoService
    {
        Task<IEnumerable<Tratamiento>> GetAllAsync();
        Task<Tratamiento?> GetByIdAsync(int id);
        Task<Tratamiento> CreateAsync(Tratamiento entity);
        Task<bool> UpdateAsync(int id, Tratamiento entity);
        Task<bool> DeleteAsync(int id);
    }
}
