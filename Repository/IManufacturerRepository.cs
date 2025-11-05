using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IManufacturerRepository
    {
        Task<IEnumerable<Manufacturer>> GetAllAsync();
        Task<Manufacturer> GetByIdAsync(int id);
        Task AddAsync(Manufacturer manufacturer);
        Task UpdateAsync(Manufacturer manufacturer);
        Task DeleteAsync(int id);
    }
}