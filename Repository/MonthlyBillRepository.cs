//using Microsoft.EntityFrameworkCore;
//using MDMS_Backend.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace MDMS_Backend.Repositories
//{
//    public class MonthlyBillRepository : IMonthlyBillRepository
//    {
//        private readonly MdmsDbContext _context;

//        public MonthlyBillRepository(MdmsDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter)
//                                 .ToListAsync();
//        }

//        public async Task<MonthlyBill> GetByIdAsync(int billId)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter)
//                                 .FirstOrDefaultAsync(m => m.BillId == billId);
//        }

//        public async Task AddAsync(MonthlyBill monthlyBill)
//        {
//            await _context.MonthlyBills.AddAsync(monthlyBill);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(MonthlyBill monthlyBill)
//        {
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(int billId)
//        {
//            var monthlyBill = await _context.MonthlyBills.FindAsync(billId);
//            if (monthlyBill != null)
//            {
//                _context.MonthlyBills.Remove(monthlyBill);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}
using Microsoft.EntityFrameworkCore;
using MDMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MDMS_Backend.Repositories
{
    public class MonthlyBillRepository : IMonthlyBillRepository
    {
        private readonly MdmsDbContext _context;

        public MonthlyBillRepository(MdmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .OrderByDescending(m => m.BillingYear)
                                 .ThenByDescending(m => m.BillingMonth)
                                 .ToListAsync();
        }

        public async Task<MonthlyBill> GetByIdAsync(int billId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .FirstOrDefaultAsync(m => m.BillId == billId);
        }

        public async Task AddAsync(MonthlyBill monthlyBill)
        {
            await _context.MonthlyBills.AddAsync(monthlyBill);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonthlyBill monthlyBill)
        {
            _context.MonthlyBills.Update(monthlyBill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int billId)
        {
            var monthlyBill = await _context.MonthlyBills.FindAsync(billId);
            if (monthlyBill != null)
            {
                _context.MonthlyBills.Remove(monthlyBill);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GenerateMonthlyBillsAsync(int month, int year)
        {
            var monthParam = new SqlParameter("@BillingMonth", month);
            var yearParam = new SqlParameter("@BillingYear", year);

            var result = await _context.Database
                .ExecuteSqlRawAsync("EXEC GenerateMonthlyBills @BillingMonth, @BillingYear",
                                  monthParam, yearParam);
            return result;
        }

        public async Task<int> GenerateMonthlyBillsFromReadingsAsync(int month, int year)
        {
            var monthParam = new SqlParameter("@BillingMonth", month);
            var yearParam = new SqlParameter("@BillingYear", year);
            var outputParam = new SqlParameter
            {
                ParameterName = "@BillsAffected",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC GenerateMonthlyBillsFromReadings @BillingMonth, @BillingYear, @BillsAffected OUTPUT",
                monthParam, yearParam, outputParam);

            return (int)outputParam.Value;
        }

        public async Task<IEnumerable<MonthlyBill>> GetByMeterAndMonthAsync(int meterId, int month, int year)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .Where(m => m.MeterId == meterId &&
                                            m.BillingMonth == month &&
                                            m.BillingYear == year)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByConsumerAsync(int consumerId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .Where(m => m.ConsumerId == consumerId)
                                 .OrderByDescending(m => m.BillingYear)
                                 .ThenByDescending(m => m.BillingMonth)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByConsumerIdAsync(int consumerId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .Where(m => m.ConsumerId == consumerId)
                                 .OrderByDescending(m => m.BillingYear)
                                 .ThenByDescending(m => m.BillingMonth)
                                 .ToListAsync();
        }

        public async Task<bool> CheckBillsExistForMonthAsync(int month, int year)
        {
            return await _context.MonthlyBills
                                 .AnyAsync(m => m.BillingMonth == month &&
                                               m.BillingYear == year);
        }
    }
}