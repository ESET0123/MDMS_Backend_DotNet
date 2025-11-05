using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface ITariffSlabRepository
    {
        Task<IEnumerable<TariffSlab>> GetAllAsync();
        Task<TariffSlab> GetByIdAsync(int id);
        Task<IEnumerable<TariffSlab>> GetByTariffIdAsync(int tariffId);
        Task AddAsync(TariffSlab tariffSlab);
        Task UpdateAsync(TariffSlab tariffSlab);
        Task DeleteAsync(int id);
    }
}
