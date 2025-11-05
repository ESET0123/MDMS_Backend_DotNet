using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Repository
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public ZoneRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Zone newZone)
        {
            await _dbcontext.Zones.AddAsync(newZone);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Zones.FirstOrDefaultAsync(n => n.ZoneId == id);

            if (deleting != null)
            {
                _dbcontext.Zones.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Zone>> GetAllAsync()
        {
            return await _dbcontext.Zones.ToListAsync();
        }

        public async Task<Zone> GetByIdAsync(int id)
        {
            return await _dbcontext.Zones.FirstOrDefaultAsync(n => n.ZoneId == id);
        }

        public async Task UpdateAsync(Zone zone)
        {
            var existingZone = await _dbcontext.Zones.FirstOrDefaultAsync(n => n.ZoneId == zone.ZoneId);

            if (existingZone == null)
            {
                return;
            }

            existingZone.ZoneName = zone.ZoneName;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
