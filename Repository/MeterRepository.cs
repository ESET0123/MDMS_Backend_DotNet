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

            int oldStatusId = existingMeter.StatusId;
            int newStatusId = meter.StatusId;
            int consumerId = existingMeter.ConsumerId;

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

            if (oldStatusId != newStatusId)
            {
                await UpdateConsumerStatusBasedOnMeters(consumerId, newStatusId);
            }
        }

        private async Task UpdateConsumerStatusBasedOnMeters(int consumerId, int meterStatusId)
        {
            const int ACTIVE_STATUS_ID = 1;
            const int INACTIVE_STATUS_ID = 2;

            var consumer = await _dbcontext.Consumers.FirstOrDefaultAsync(c => c.ConsumerId == consumerId);
            if (consumer == null) return;

            var consumerMeters = await _dbcontext.Meters
                .Where(m => m.ConsumerId == consumerId)
                .ToListAsync();

            if (meterStatusId == INACTIVE_STATUS_ID)
            {
                bool hasActiveMeters = consumerMeters.Any(m => m.StatusId == ACTIVE_STATUS_ID);

                if (!hasActiveMeters && consumer.StatusId == ACTIVE_STATUS_ID)
                {
                    consumer.StatusId = INACTIVE_STATUS_ID;
                    await _dbcontext.SaveChangesAsync();
                }
            }
            else if (meterStatusId == ACTIVE_STATUS_ID)
            {
                if (consumer.StatusId == INACTIVE_STATUS_ID)
                {
                    consumer.StatusId = ACTIVE_STATUS_ID;
                    await _dbcontext.SaveChangesAsync();
                }
            }
        }

        //public async Task<bool> ConsumerHasActiveMeters(int consumerId)
        //{
        //    const int ACTIVE_STATUS_ID = 1;
        //    return await _dbcontext.Meters
        //        .AnyAsync(m => m.ConsumerId == consumerId && m.StatusId == ACTIVE_STATUS_ID);
        //}
    }
}

//using MDMS_Backend.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Linq;

//namespace MDMS_Backend.Repository
//{
//    public class MeterRepository : IMeterRepository
//    {
//        private readonly MdmsDbContext _dbcontext;

//        public MeterRepository(MdmsDbContext dbcontext)
//        {
//            _dbcontext = dbcontext;
//        }

//        public async Task AddAsync(Meter newMeter)
//        {
//            await _dbcontext.Meters.AddAsync(newMeter);
//            await _dbcontext.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(int id)
//        {
//            var deleting = await _dbcontext.Meters.FirstOrDefaultAsync(n => n.MeterId == id);

//            if (deleting != null)
//            {
//                _dbcontext.Meters.Remove(deleting);
//                await _dbcontext.SaveChangesAsync();
//            }
//        }

//        public async Task<IEnumerable<Meter>> GetAllAsync()
//        {
//            return await _dbcontext.Meters
//                .Include(m => m.Consumer)
//                .Include(m => m.Dtr)
//                .Include(m => m.Manufacturer)
//                .Include(m => m.Tariff)
//                .Include(m => m.Status)
//                .ToListAsync();
//        }

//        public async Task<Meter> GetByIdAsync(int id)
//        {
//            return await _dbcontext.Meters
//                .Include(m => m.Consumer)
//                .Include(m => m.Dtr)
//                .Include(m => m.Manufacturer)
//                .Include(m => m.Tariff)
//                .Include(m => m.Status)
//                .FirstOrDefaultAsync(n => n.MeterId == id);
//        }

//        public async Task UpdateAsync(Meter meter)
//        {
//            var existingMeter = await _dbcontext.Meters.FirstOrDefaultAsync(n => n.MeterId == meter.MeterId);

//            if (existingMeter == null)
//            {
//                return;
//            }

//            // Update modifiable fields
//            existingMeter.ConsumerId = meter.ConsumerId;
//            existingMeter.Dtrid = meter.Dtrid;
//            existingMeter.Ipaddress = meter.Ipaddress;
//            existingMeter.Iccid = meter.Iccid;
//            existingMeter.Imsi = meter.Imsi;
//            existingMeter.ManufacturerId = meter.ManufacturerId;
//            existingMeter.Firmware = meter.Firmware;
//            existingMeter.TariffId = meter.TariffId;
//            existingMeter.InstallDate = meter.InstallDate;
//            existingMeter.StatusId = meter.StatusId;
//            existingMeter.LatestReading = meter.LatestReading;

//            await _dbcontext.SaveChangesAsync();
//        }
//    }
//}