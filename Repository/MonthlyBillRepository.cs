using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDMS_Backend.Repositories
{
    public class MonthlyBillRepository : IMonthlyBillRepository
    {
        private readonly MdmsDbContext _context; // Replace with your actual DbContext name

        public MonthlyBillRepository(MdmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .OrderByDescending(b => b.BillingYear)
                .ThenByDescending(b => b.BillingMonth)
                .ToListAsync();
        }

        public async Task<MonthlyBill> GetByIdAsync(int billId)
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .FirstOrDefaultAsync(b => b.BillId == billId);
        }

        public async Task<IEnumerable<MonthlyBill>> GetByMeterIdAsync(int meterId)
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .Where(b => b.MeterId == meterId)
                .OrderByDescending(b => b.BillingYear)
                .ThenByDescending(b => b.BillingMonth)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByConsumerIdAsync(int consumerId)
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .Where(b => b.ConsumerId == consumerId)
                .OrderByDescending(b => b.BillingYear)
                .ThenByDescending(b => b.BillingMonth)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByMonthAsync(int month, int year)
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .Where(b => b.BillingMonth == month && b.BillingYear == year)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByMonthOnlyAsync(int month)
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .Where(b => b.BillingMonth == month)
                .OrderByDescending(b => b.BillingYear)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByYearAsync(int year)
        {
            return await _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .Where(b => b.BillingYear == year)
                .OrderBy(b => b.BillingMonth)
                .ToListAsync();
        }

        public async Task<MonthlyBill> CreateAsync(MonthlyBill bill)
        {
            _context.MonthlyBills.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task<MonthlyBill> UpdateAsync(MonthlyBill bill)
        {
            _context.Entry(bill).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task<bool> DeleteAsync(int billId)
        {
            var bill = await _context.MonthlyBills.FindAsync(billId);
            if (bill == null)
                return false;

            _context.MonthlyBills.Remove(bill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MonthlyBill>> GetFilteredBillsAsync(
            int? meterId = null,
            int? consumerId = null,
            int? month = null,
            int? year = null,
            string status = null)
        {
            var query = _context.MonthlyBills
                .Include(b => b.Consumer)
                .Include(b => b.Meter)
                .AsQueryable();

            if (meterId.HasValue)
                query = query.Where(b => b.MeterId == meterId.Value);

            if (consumerId.HasValue)
                query = query.Where(b => b.ConsumerId == consumerId.Value);

            if (month.HasValue)
                query = query.Where(b => b.BillingMonth == month.Value);

            if (year.HasValue)
                query = query.Where(b => b.BillingYear == year.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.BillStatus == status);

            return await query
                .OrderByDescending(b => b.BillingYear)
                .ThenByDescending(b => b.BillingMonth)
                .ToListAsync();
        }
    }
}