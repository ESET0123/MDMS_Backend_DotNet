using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IConsumerRepository
    {
        Task<IEnumerable<Consumer>> GetAllAsync();
        Task<Consumer> GetByIdAsync(int consumerId);
        Task<Consumer> GetByEmailAsync(string email);
        Task AddAsync(Consumer consumer);
        Task UpdateAsync(Consumer consumer);
        Task DeleteAsync(int consumerId);
        Task<IEnumerable<int>> GetMeterIdsByConsumerIdAsync(int consumerId);

    }
}