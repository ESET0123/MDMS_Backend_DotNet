using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MDMS_Backend.Repository
{
    public class MeterRepository : IMeterRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public MeterRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Meter newMeter)
        {
            await _dbcontext.Meters.AddAsync(newMeter);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Meters.FirstOrDefaultAsync(n => n.MeterId == id);

            if (deleting != null)
            {
                _dbcontext.Meters.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Meter>> GetAllAsync()
        {
            // Include related entities for richer data display in the list view
            return await _dbcontext.Meters
                .Include(m => m.Consumer)
                .Include(m => m.Dtr)
                .Include(m => m.Manufacturer)
                .Include(m => m.Tariff)
                .Include(m => m.Status)
                .ToListAsync();
        }

        public async Task<Meter> GetByIdAsync(int id)
        {
            // Include related entities for detailed view
            return await _dbcontext.Meters
                .Include(m => m.Consumer)
                .Include(m => m.Dtr)
                .Include(m => m.Manufacturer)
                .Include(m => m.Tariff)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(n => n.MeterId == id);
        }

        public async Task UpdateAsync(Meter meter)
        {
            var existingMeter = await _dbcontext.Meters.FirstOrDefaultAsync(n => n.MeterId == meter.MeterId);

            if (existingMeter == null)
            {
                return;
            }

            // Update modifiable fields
            existingMeter.ConsumerId = meter.ConsumerId;
            existingMeter.Dtrid = meter.Dtrid;
            existingMeter.Ipaddress = meter.Ipaddress;
            existingMeter.Iccid = meter.Iccid;
            existingMeter.Imsi = meter.Imsi;
            existingMeter.ManufacturerId = meter.ManufacturerId;
            existingMeter.Firmware = meter.Firmware;
            existingMeter.TariffId = meter.TariffId;
            existingMeter.InstallDate = meter.InstallDate;
            existingMeter.StatusId = meter.StatusId;
            existingMeter.LatestReading = meter.LatestReading;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
