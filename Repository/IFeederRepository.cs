using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IFeederRepository
    {
        Task<IEnumerable<Feeder>> GetAllAsync();
        Task<Feeder> GetByIdAsync(int id);
        Task AddAsync(Feeder feeder);
        Task UpdateAsync(Feeder feeder);
        Task DeleteAsync(int id);
    }
}
