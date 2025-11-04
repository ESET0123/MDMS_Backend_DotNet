using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Repository
{
    public class FeederRepository : IFeederRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public FeederRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Feeder newFeeder)
        {
            await _dbcontext.Feeders.AddAsync(newFeeder);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Feeders.FirstOrDefaultAsync(n => n.FeederId == id);

            if (deleting != null)
            {
                _dbcontext.Feeders.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Feeder>> GetAllAsync()
        {
            // Include the related Substation for a richer context
            return await _dbcontext.Feeders.Include(f => f.Substation).ToListAsync();
        }

        public async Task<Feeder> GetByIdAsync(int id)
        {
            // Include the related Substation for a richer context
            return await _dbcontext.Feeders.Include(f => f.Substation).FirstOrDefaultAsync(n => n.FeederId == id);
        }

        public async Task UpdateAsync(Feeder feeder)
        {
            var existingFeeder = await _dbcontext.Feeders.FirstOrDefaultAsync(n => n.FeederId == feeder.FeederId);

            if (existingFeeder == null)
            {
                return;
            }

            // Update modifiable fields
            existingFeeder.FeederName = feeder.FeederName;
            existingFeeder.SubstationId = feeder.SubstationId;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
