using WebApi.Models;

namespace WebApi.Interface
{
    public interface IMedicamentoService
    {
        Task<IEnumerable<Medicamento>> GetAllAsync();
        Task<Medicamento?> GetByIdAsync(int id);
        Task<Medicamento> CreateAsync(Medicamento entity);
        Task<bool> UpdateAsync(int id, Medicamento entity);
        Task<bool> DeleteAsync(int id);
    }
}
