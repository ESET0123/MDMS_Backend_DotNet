using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface ITariffSlabRepository
    {
        Task<IEnumerable<TariffSlab>> GetAllAsync();
        Task<TariffSlab> GetByIdAsync(int id);
        // Often useful to get all slabs for a specific tariff
        Task<IEnumerable<TariffSlab>> GetByTariffIdAsync(int tariffId);
        Task AddAsync(TariffSlab tariffSlab);
        Task UpdateAsync(TariffSlab tariffSlab);
        Task DeleteAsync(int id);
    }
}
