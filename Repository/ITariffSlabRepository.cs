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

        Task<bool> HasOverlappingSlabAsync(TariffSlab slab, int? excludeSlabId = null);
        Task<IEnumerable<TariffSlab>> GetPotentialOverlapsAsync(int tariffId, DateOnly fromDate, DateOnly toDate, decimal fromKwh, decimal toKwh, int? excludeSlabId = null);
    }
}
