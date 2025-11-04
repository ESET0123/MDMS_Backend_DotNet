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
            // Include the related Tariff information
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
            // Include the related Tariff information
            return await _dbcontext.TariffSlabs.Include(ts => ts.Tariff).FirstOrDefaultAsync(n => n.SlabId == id);
        }

        public async Task UpdateAsync(TariffSlab tariffSlab)
        {
            var existingTariffSlab = await _dbcontext.TariffSlabs.FirstOrDefaultAsync(n => n.SlabId == tariffSlab.SlabId);

            if (existingTariffSlab == null)
            {
                return;
            }

            // Update modifiable fields
            existingTariffSlab.TariffId = tariffSlab.TariffId;
            existingTariffSlab.FromKwh = tariffSlab.FromKwh;
            existingTariffSlab.ToKwh = tariffSlab.ToKwh;
            existingTariffSlab.RatePerKwh = tariffSlab.RatePerKwh;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
