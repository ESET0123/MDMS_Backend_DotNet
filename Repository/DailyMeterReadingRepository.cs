using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Repository
{
    public class DailyMeterReadingRepository : IDailyMeterReadingRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public DailyMeterReadingRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<DailyMeterReading>> GetAllAsync()
        {
            return await _dbcontext.DailyMeterReadings
                .Include(d => d.Meter)
                .Include(d => d.TodRule)
                .OrderByDescending(d => d.ReadingDate)
                .ThenBy(d => d.StartTime)
                .ToListAsync();
        }

        public async Task<DailyMeterReading> GetByIdAsync(int id)
        {
            return await _dbcontext.DailyMeterReadings
                .Include(d => d.Meter)
                .Include(d => d.TodRule)
                .FirstOrDefaultAsync(d => d.ReadingId == id);
        }

        public async Task<IEnumerable<DailyMeterReading>> GetByMeterAsync(int meterId)
        {
            return await _dbcontext.DailyMeterReadings
                .Include(d => d.Meter)
                .Include(d => d.TodRule)
                .Where(d => d.MeterId == meterId)
                .OrderByDescending(d => d.ReadingDate)
                .ThenBy(d => d.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyMeterReading>> GetByMeterAndDateRangeAsync(int meterId, DateOnly startDate, DateOnly endDate)
        {
            return await _dbcontext.DailyMeterReadings
                .Include(d => d.Meter)
                .Include(d => d.TodRule)
                .Where(d => d.MeterId == meterId && d.ReadingDate >= startDate && d.ReadingDate <= endDate)
                .OrderBy(d => d.ReadingDate)
                .ThenBy(d => d.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyMeterReading>> GetByDateAsync(DateOnly date)
        {
            return await _dbcontext.DailyMeterReadings
                .Include(d => d.Meter)
                .Include(d => d.TodRule)
                .Where(d => d.ReadingDate == date)
                .OrderBy(d => d.MeterId)
                .ThenBy(d => d.StartTime)
                .ToListAsync();
        }

        public async Task AddAsync(DailyMeterReading reading)
        {
            await _dbcontext.DailyMeterReadings.AddAsync(reading);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<DailyMeterReading> readings)
        {
            await _dbcontext.DailyMeterReadings.AddRangeAsync(readings);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(DailyMeterReading reading)
        {
            var existing = await _dbcontext.DailyMeterReadings.FirstOrDefaultAsync(d => d.ReadingId == reading.ReadingId);
            if (existing == null) return;

            existing.PreviousReading = reading.PreviousReading;
            existing.CurrentReading = reading.CurrentReading;
            existing.ConsumptionKwh = reading.ConsumptionKwh;
            existing.BaseRate = reading.BaseRate;
            existing.SurgeChargePercent = reading.SurgeChargePercent;
            existing.DiscountPercent = reading.DiscountPercent;
            existing.EffectiveRate = reading.EffectiveRate;
            existing.Amount = reading.Amount;

            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.DailyMeterReadings.FirstOrDefaultAsync(d => d.ReadingId == id);
            if (deleting != null)
            {
                _dbcontext.DailyMeterReadings.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}
