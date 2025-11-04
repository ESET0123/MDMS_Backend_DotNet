using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface ISubstationRepository
    {
        Task<IEnumerable<Substation>> GetAllAsync();
        Task<Substation> GetByIdAsync(int id);
        Task AddAsync(Substation substation);
        Task UpdateAsync(Substation substation);
        Task DeleteAsync(int id);
    }
}
