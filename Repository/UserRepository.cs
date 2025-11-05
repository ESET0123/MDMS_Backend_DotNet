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

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.PasswordHashed = user.PasswordHashed;
            existingUser.RoleId = user.RoleId;
            existingUser.LastLogin = user.LastLogin;
            existingUser.Active = user.Active;

            await _dbcontext.SaveChangesAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _dbcontext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

    }
}
