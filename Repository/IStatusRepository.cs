using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IStatusRepository
    {
        Task<IEnumerable<Status>> GetAllAsync();
        Task AddAsync(Status status);
        Task<Status> GetByIdAsync(int id);
        Task UpdateAsync(Status status);
        Task DeleteAsync(int id);
    }
}
