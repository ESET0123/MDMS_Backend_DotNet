using MDMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Repositories
{
    public interface IMonthlyBillRepository
    {
        // Get all bills
        Task<IEnumerable<MonthlyBill>> GetAllAsync();

        // Get bill by ID
        Task<MonthlyBill> GetByIdAsync(int billId);

        // Get bills by Meter ID
        Task<IEnumerable<MonthlyBill>> GetByMeterIdAsync(int meterId);

        // Get bills by Consumer ID
        Task<IEnumerable<MonthlyBill>> GetByConsumerIdAsync(int consumerId);

        // Get bills by Month and Year
        Task<IEnumerable<MonthlyBill>> GetByMonthAsync(int month, int year);

        // Get bills by Month only (all years)
        Task<IEnumerable<MonthlyBill>> GetByMonthOnlyAsync(int month);

        // Get bills by Year only
        Task<IEnumerable<MonthlyBill>> GetByYearAsync(int year);

        // Create new bill
        Task<MonthlyBill> CreateAsync(MonthlyBill bill);

        // Update existing bill
        Task<MonthlyBill> UpdateAsync(MonthlyBill bill);

        // Delete bill
        Task<bool> DeleteAsync(int billId);

        // Get bills with filters (combined)
        Task<IEnumerable<MonthlyBill>> GetFilteredBillsAsync(
            int? meterId = null,
            int? consumerId = null,
            int? month = null,
            int? year = null,
            string status = null);
    }
}