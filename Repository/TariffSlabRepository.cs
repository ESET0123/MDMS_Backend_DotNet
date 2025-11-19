using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MDMS_Backend.Repository
{
    public class TariffSlabRepository : ITariffSlabRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public TariffSlabRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(TariffSlab newTariffSlab)
        {
            await _dbcontext.TariffSlabs.AddAsync(newTariffSlab);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.TariffSlabs.FirstOrDefaultAsync(n => n.SlabId == id);

            if (deleting != null)
            {
                _dbcontext.TariffSlabs.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TariffSlab>> GetAllAsync()
        {
            return await _dbcontext.TariffSlabs.Include(ts => ts.Tariff).ToListAsync();
        }

        public async Task<IEnumerable<TariffSlab>> GetByTariffIdAsync(int tariffId)
        {
            return await _dbcontext.TariffSlabs
                .Include(ts => ts.Tariff)
                .Where(ts => ts.TariffId == tariffId)
                .ToListAsync();
        }

        public async Task<TariffSlab> GetByIdAsync(int id)
        {
            return await _dbcontext.TariffSlabs.Include(ts => ts.Tariff).FirstOrDefaultAsync(n => n.SlabId == id);
        }

        public async Task UpdateAsync(TariffSlab tariffSlab)
        {
            var existingTariffSlab = await _dbcontext.TariffSlabs.FirstOrDefaultAsync(n => n.SlabId == tariffSlab.SlabId);
            if (existingTariffSlab == null)
            {
                return;
            }

            existingTariffSlab.TariffId = tariffSlab.TariffId;
            existingTariffSlab.FromKwh = tariffSlab.FromKwh;
            existingTariffSlab.ToKwh = tariffSlab.ToKwh;
            existingTariffSlab.RatePerKwh = tariffSlab.RatePerKwh;
            existingTariffSlab.FromDate = tariffSlab.FromDate;
            existingTariffSlab.ToDate = tariffSlab.ToDate;

            await _dbcontext.SaveChangesAsync();
        }

        public async Task<bool> HasOverlappingSlabAsync(TariffSlab slab, int? excludeSlabId = null)
        {
            var query = _dbcontext.TariffSlabs
                .Where(ts => ts.TariffId == slab.TariffId)
                // Check for date range overlap
                .Where(ts => ts.FromDate <= slab.ToDate && ts.ToDate >= slab.FromDate)
                // Check for consumption range overlap
                .Where(ts => ts.FromKwh <= slab.ToKwh && ts.ToKwh >= slab.FromKwh);

            if (excludeSlabId.HasValue)
            {
                query = query.Where(ts => ts.SlabId != excludeSlabId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<TariffSlab>> GetPotentialOverlapsAsync(int tariffId, DateOnly fromDate, DateOnly toDate, decimal fromKwh, decimal toKwh, int? excludeSlabId = null)
        {
            var query = _dbcontext.TariffSlabs
                .Where(ts => ts.TariffId == tariffId)
                .Where(ts => ts.FromDate <= toDate && ts.ToDate >= fromDate)
                .Where(ts => ts.FromKwh <= toKwh && ts.ToKwh >= fromKwh);

            if (excludeSlabId.HasValue)
            {
                query = query.Where(ts => ts.SlabId != excludeSlabId.Value);
            }

            return await query.Include(ts => ts.Tariff).ToListAsync();
        }
    }
}
