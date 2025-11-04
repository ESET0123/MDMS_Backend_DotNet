using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IZoneRepository
    {
        Task<IEnumerable<Zone>> GetAllAsync();
        Task AddAsync(Zone zone);
        Task<Zone> GetByIdAsync(int id);
        Task UpdateAsync(Zone zone);
        Task DeleteAsync(int id);
    }
}
