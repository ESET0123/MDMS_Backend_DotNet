
using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MDMS_Backend.Repository
{
    // ============= TOD Rule Repository =============


    public class TodRuleRepository : ITodRuleRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public TodRuleRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<TodRule>> GetAllAsync()
        {
            return await _dbcontext.TodRules
                .Include(t => t.Tariff)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<TodRule> GetByIdAsync(int id)
        {
            return await _dbcontext.TodRules
                .Include(t => t.Tariff)
                .FirstOrDefaultAsync(t => t.TodRuleId == id);
        }

        public async Task<IEnumerable<TodRule>> GetActiveRulesAsync()
        {
            return await _dbcontext.TodRules
                .Include(t => t.Tariff)
                .Where(t => t.IsActive)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodRule>> GetRulesByTariffAsync(int tariffId)
        {
            return await _dbcontext.TodRules
                .Include(t => t.Tariff)
                .Where(t => t.TariffId == tariffId)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task AddAsync(TodRule todRule)
        {
            await _dbcontext.TodRules.AddAsync(todRule);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodRule todRule)
        {
            var existing = await _dbcontext.TodRules.FirstOrDefaultAsync(t => t.TodRuleId == todRule.TodRuleId);
            if (existing == null) return;

            existing.RuleName = todRule.RuleName;
            existing.StartTime = todRule.StartTime;
            existing.EndTime = todRule.EndTime;
            existing.TariffId = todRule.TariffId;
            existing.SurgeChargePercent = todRule.SurgeChargePercent;
            existing.DiscountPercent = todRule.DiscountPercent;
            existing.IsActive = todRule.IsActive;
            existing.UpdatedAt = todRule.UpdatedAt;
            existing.UpdatedBy = todRule.UpdatedBy;

            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.TodRules.FirstOrDefaultAsync(t => t.TodRuleId == id);
            if (deleting != null)
            {
                _dbcontext.TodRules.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}