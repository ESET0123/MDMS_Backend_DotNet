using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Repository
{
    public class TariffRepository : ITariffRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public TariffRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Tariff newTariff)
        {
            await _dbcontext.Tariffs.AddAsync(newTariff);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Tariffs.FirstOrDefaultAsync(n => n.TariffId == id);

            if (deleting != null)
            {
                _dbcontext.Tariffs.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tariff>> GetAllAsync()
        {
            // We usually don't include Meters or TariffSlabs in a general list view
            return await _dbcontext.Tariffs.ToListAsync();
        }

        public async Task<Tariff> GetByIdAsync(int id)
        {
            // If you need slabs/meters for a detailed view, add .Include() here
            return await _dbcontext.Tariffs.FirstOrDefaultAsync(n => n.TariffId == id);
        }

        public async Task UpdateAsync(Tariff tariff)
        {
            var existingTariff = await _dbcontext.Tariffs.FirstOrDefaultAsync(n => n.TariffId == tariff.TariffId);

            if (existingTariff == null)
            {
                return;
            }

            // Update modifiable fields
            existingTariff.Name = tariff.Name;
            existingTariff.EffectiveFrom = tariff.EffectiveFrom;
            existingTariff.EffectiveTo = tariff.EffectiveTo;
            existingTariff.BaseRate = tariff.BaseRate;
            existingTariff.TaxRate = tariff.TaxRate;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
