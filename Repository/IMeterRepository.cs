using MDMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Repository
{
    public interface IMeterRepository
    {
        Task<IEnumerable<Meter>> GetAllAsync();
        Task<Meter> GetByIdAsync(int id);
        Task AddAsync(Meter meter);
        Task UpdateAsync(Meter meter);
        Task DeleteAsync(int id);

    }
}
