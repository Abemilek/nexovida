using WebApi.Models;

namespace WebApi.Interface
{
    public interface IFamiliarService
    {
        Task<IEnumerable<Familiar>> GetAllAsync();
        Task<Familiar?> GetByIdAsync(int id);
        Task<Familiar> CreateAsync(Familiar entity);
        Task<bool> UpdateAsync(int id, Familiar entity);
        Task<bool> DeleteAsync(int id);
    }
}
