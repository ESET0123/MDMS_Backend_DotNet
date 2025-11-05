using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByUserIdAsync(string userId);

        Task<User> GetUserByUsernameAsync(string username);
        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(string userId);
    }
}
