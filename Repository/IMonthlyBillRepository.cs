//using MDMS_Backend.Controllers;
//using MDMS_Backend.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace MDMS_Backend.Repositories
//{
//    public interface IMonthlyBillRepository
//    {
//        Task<IEnumerable<MonthlyBill>> GetAllAsync();
//        Task<MonthlyBill> GetByIdAsync(int billId);
//        Task AddAsync(MonthlyBill monthlyBill);
//        Task UpdateAsync(MonthlyBill monthlyBill);
//        Task DeleteAsync(int billId);
//    }
//}
using MDMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Repositories
{
    public interface IMonthlyBillRepository
    {
        Task<IEnumerable<MonthlyBill>> GetAllAsync();
        Task<MonthlyBill> GetByIdAsync(int billId);
        Task AddAsync(MonthlyBill monthlyBill);
        Task UpdateAsync(MonthlyBill monthlyBill);
        Task DeleteAsync(int billId);

        // New methods for bill generation
        Task<int> GenerateMonthlyBillsAsync(int month, int year);
        Task<IEnumerable<MonthlyBill>> GetByMeterAndMonthAsync(int meterId, int month, int year);
        Task<IEnumerable<MonthlyBill>> GetByConsumerAsync(int consumerId);
        Task<bool> CheckBillsExistForMonthAsync(int month, int year);
    }
}