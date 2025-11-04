using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MDMS_Backend.Repository
{
    public class DtrRepository : IDtrRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public DtrRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Dtr newDtr)
        {
            await _dbcontext.Dtrs.AddAsync(newDtr);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Dtrs.FirstOrDefaultAsync(n => n.Dtrid == id);

            if (deleting != null)
            {
                _dbcontext.Dtrs.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Dtr>> GetAllAsync()
        {
            // Include the related Feeder information
            return await _dbcontext.Dtrs.Include(d => d.Feeder).ToListAsync();
        }

        public async Task<Dtr> GetByIdAsync(int id)
        {
            // Include the related Feeder information
            return await _dbcontext.Dtrs.Include(d => d.Feeder).FirstOrDefaultAsync(n => n.Dtrid == id);
        }

        public async Task UpdateAsync(Dtr dtr)
        {
            var existingDtr = await _dbcontext.Dtrs.FirstOrDefaultAsync(n => n.Dtrid == dtr.Dtrid);

            if (existingDtr == null)
            {
                return;
            }

            // Update modifiable fields
            existingDtr.Dtrname = dtr.Dtrname;
            existingDtr.FeederId = dtr.FeederId;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
