using Microsoft.EntityFrameworkCore;
using MDMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                                 .Include(m => m.Meter)
                                 .ToListAsync();
        }

        public async Task<MonthlyBill> GetByIdAsync(int billId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter)
                                 .FirstOrDefaultAsync(m => m.BillId == billId);
        }

        public async Task AddAsync(MonthlyBill monthlyBill)
        {
            await _context.MonthlyBills.AddAsync(monthlyBill);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonthlyBill monthlyBill)
        {
            //_context.Entry(monthlyBill).State = EntityState.Modified;
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
    }
}
