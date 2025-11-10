using MDMS_Backend.Models;

namespace MDMS_Backend.Repository
{
    public interface IMonthlyBillRepository
    {
        Task<IEnumerable<MonthlyBill>> GetAllAsync();
        Task<MonthlyBill> GetByIdAsync(int id);
        Task<IEnumerable<MonthlyBill>> GetByConsumerAsync(int consumerId);
        Task<MonthlyBill> GetByMeterAndMonthAsync(int meterId, int year, int month);
        Task<IEnumerable<MonthlyBill>> GetByMonthAsync(int year, int month);
        Task<IEnumerable<MonthlyBill>> GetPendingBillsAsync();
        Task AddAsync(MonthlyBill bill);
        Task UpdateAsync(MonthlyBill bill);
        Task DeleteAsync(int id);
    }
}
