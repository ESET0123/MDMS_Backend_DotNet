using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();

        Task AddAsync(Role role);

        Task<Role> GetByIdAsync(int id);

        Task UpdateAsync(Role role);

        Task DeleteAsync(int id);
    }
}
