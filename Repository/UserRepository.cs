using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MDMS_Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public UserRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(User newUser)
        {
            await _dbcontext.Users.AddAsync(newUser);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var deleting = await _dbcontext.Users.FirstOrDefaultAsync(n => n.UserId == userId);

            if (deleting != null)
            {
                _dbcontext.Users.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbcontext.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<User> GetByUserIdAsync(string userId)
        {
            return await _dbcontext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(n => n.UserId == userId);
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _dbcontext.Users.FirstOrDefaultAsync(n => n.UserId == user.UserId);

            if (existingUser == null)
            {
                return;
            }

            // Update all modifiable fields
            existingUser.Username = user.Username;
            // Note: DisplayName was removed in your latest User model definition
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            // Updated property name here:
            existingUser.PasswordHashed = user.PasswordHashed;
            existingUser.RoleId = user.RoleId;
            existingUser.LastLogin = user.LastLogin;
            existingUser.Active = user.Active;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
