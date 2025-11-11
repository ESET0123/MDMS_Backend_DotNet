using System.Collections.Generic;
using System.Threading.Tasks;
using MDMS_Backend.Models;

namespace MDMS_Backend.Repositories
{
    public interface IMonthlyBillRepository
    {
        Task<IEnumerable<MonthlyBill>> GetAllAsync();
        Task<MonthlyBill> GetByIdAsync(int billId);
        Task AddAsync(MonthlyBill monthlyBill);
        Task UpdateAsync(MonthlyBill monthlyBill);
        Task DeleteAsync(int billId);
    }
}
