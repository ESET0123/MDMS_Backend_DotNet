using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IDailyMeterReadingRepository
    {
        Task<IEnumerable<DailyMeterReading>> GetAllAsync();
        Task<DailyMeterReading> GetByIdAsync(int id);
        Task<IEnumerable<DailyMeterReading>> GetByMeterAsync(int meterId);
        Task<IEnumerable<DailyMeterReading>> GetByMeterAndDateRangeAsync(int meterId, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<DailyMeterReading>> GetByDateAsync(DateOnly date);
        Task AddAsync(DailyMeterReading reading);
        Task AddRangeAsync(IEnumerable<DailyMeterReading> readings);
        Task UpdateAsync(DailyMeterReading reading);
        Task DeleteAsync(int id);
    }
}
