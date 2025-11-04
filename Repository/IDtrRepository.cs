using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IDtrRepository
    {
        Task<IEnumerable<Dtr>> GetAllAsync();
        Task<Dtr> GetByIdAsync(int id);
        Task AddAsync(Dtr dtr);
        Task UpdateAsync(Dtr dtr);
        Task DeleteAsync(int id);
    }
}
