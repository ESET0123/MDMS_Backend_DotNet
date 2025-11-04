
using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MdmsDbContext _dbcontext;
        public RoleRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Role newrole)
        {
            await _dbcontext.Roles.AddAsync(newrole);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Roles.Where(n => n.RoleId == id).FirstOrDefaultAsync();

            _dbcontext.Roles.Remove(deleting);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _dbcontext.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _dbcontext.Roles.Where(n => n.RoleId == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            var existingRole = await _dbcontext.Roles.Where(n => n.RoleId == role.RoleId).FirstOrDefaultAsync();

            if (existingRole == null)
            {
                return;
            }

            existingRole.RoleId = role.RoleId;
            existingRole.RoleName = role.RoleName;
            existingRole.Abbreviation = role.Abbreviation;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
