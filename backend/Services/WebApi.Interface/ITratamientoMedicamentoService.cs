using WebApi.Models;

namespace WebApi.Interface
{
    public interface ITratamientoMedicamentoService
    {
        Task<IEnumerable<TratamientoMedicamento>> GetAllAsync();
        Task<TratamientoMedicamento?> GetByIdAsync(int id);
        Task<TratamientoMedicamento> CreateAsync(TratamientoMedicamento entity);
        Task<bool> UpdateAsync(int id, TratamientoMedicamento entity);
        Task<bool> DeleteAsync(int id);
    }
}
