using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Repository
{
    public class StatusRepository : IStatusRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public StatusRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<Status>> GetAllAsync()
        {
            // Optionally include related entities if needed
            // Example: Include Consumers and Meters for context
            return await _dbcontext.Statuses
                                   .Include(s => s.Consumers)
                                   .Include(s => s.Meters)
                                   .ToListAsync();
        }

        public async Task<Status?> GetByIdAsync(int id)
        {
            return await _dbcontext.Statuses
                                   .Include(s => s.Consumers)
                                   .Include(s => s.Meters)
                                   .FirstOrDefaultAsync(s => s.StatusId == id);
        }

        public async Task AddAsync(Status status)
        {
            await _dbcontext.Statuses.AddAsync(status);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Status status)
        {
            var existingStatus = await _dbcontext.Statuses.FirstOrDefaultAsync(s => s.StatusId == status.StatusId);

            if (existingStatus == null)
            {
                return;
            }

            existingStatus.Name = status.Name;
            // Relationships (Consumers, Meters) typically not directly updated here

            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var status = await _dbcontext.Statuses.FirstOrDefaultAsync(s => s.StatusId == id);
            if (status != null)
            {
                _dbcontext.Statuses.Remove(status);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}
