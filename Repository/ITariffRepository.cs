using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface ITariffRepository
    {
        Task<IEnumerable<Tariff>> GetAllAsync();
        Task<Tariff> GetByIdAsync(int id);
        Task AddAsync(Tariff tariff);
        Task UpdateAsync(Tariff tariff);
        Task DeleteAsync(int id);
    }
}
