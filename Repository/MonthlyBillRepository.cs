using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Repository
{
    public class MonthlyBillRepository : IMonthlyBillRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public MonthlyBillRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
        {
            return await _dbcontext.MonthlyBills
                .Include(m => m.Meter)
                .Include(m => m.Consumer)
                .OrderByDescending(m => m.BillingYear)
                .ThenByDescending(m => m.BillingMonth)
                .ToListAsync();
        }

        public async Task<MonthlyBill> GetByIdAsync(int id)
        {
            return await _dbcontext.MonthlyBills
                .Include(m => m.Meter)
                .Include(m => m.Consumer)
                .FirstOrDefaultAsync(m => m.BillId == id);
        }

        public async Task<IEnumerable<MonthlyBill>> GetByConsumerAsync(int consumerId)
        {
            return await _dbcontext.MonthlyBills
                .Include(m => m.Meter)
                .Include(m => m.Consumer)
                .Where(m => m.ConsumerId == consumerId)
                .OrderByDescending(m => m.BillingYear)
                .ThenByDescending(m => m.BillingMonth)
                .ToListAsync();
        }

        public async Task<MonthlyBill> GetByMeterAndMonthAsync(int meterId, int year, int month)
        {
            return await _dbcontext.MonthlyBills
                .Include(m => m.Meter)
                .Include(m => m.Consumer)
                .FirstOrDefaultAsync(m => m.MeterId == meterId && m.BillingYear == year && m.BillingMonth == month);
        }

        public async Task<IEnumerable<MonthlyBill>> GetByMonthAsync(int year, int month)
        {
            return await _dbcontext.MonthlyBills
                .Include(m => m.Meter)
                .Include(m => m.Consumer)
                .Where(m => m.BillingYear == year && m.BillingMonth == month)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetPendingBillsAsync()
        {
            return await _dbcontext.MonthlyBills
                .Include(m => m.Meter)
                .Include(m => m.Consumer)
                .Where(m => m.BillStatus == "Pending")
                .OrderByDescending(m => m.BillingYear)
                .ThenByDescending(m => m.BillingMonth)
                .ToListAsync();
        }

        public async Task AddAsync(MonthlyBill bill)
        {
            await _dbcontext.MonthlyBills.AddAsync(bill);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonthlyBill bill)
        {
            var existing = await _dbcontext.MonthlyBills.FirstOrDefaultAsync(m => m.BillId == bill.BillId);
            if (existing == null) return;

            existing.BillStatus = bill.BillStatus;
            existing.PaidDate = bill.PaidDate;
            existing.TotalAmount = bill.TotalAmount;

            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.MonthlyBills.FirstOrDefaultAsync(m => m.BillId == id);
            if (deleting != null)
            {
                _dbcontext.MonthlyBills.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}