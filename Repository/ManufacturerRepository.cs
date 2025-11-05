using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Repository
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public ManufacturerRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Manufacturer newManufacturer)
        {
            await _dbcontext.Manufacturers.AddAsync(newManufacturer);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Manufacturers.FirstOrDefaultAsync(m => m.ManufacturerId == id);
            if (deleting != null)
            {
                _dbcontext.Manufacturers.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            // Include the related Meters collection
            return await _dbcontext.Manufacturers.Include(m => m.Meters).ToListAsync();
        }

        public async Task<Manufacturer> GetByIdAsync(int id)
        {
            // Include the related Meters collection
            return await _dbcontext.Manufacturers
                .Include(m => m.Meters)
                .FirstOrDefaultAsync(m => m.ManufacturerId == id);
        }

        public async Task UpdateAsync(Manufacturer manufacturer)
        {
            var existingManufacturer = await _dbcontext.Manufacturers
                .FirstOrDefaultAsync(m => m.ManufacturerId == manufacturer.ManufacturerId);

            if (existingManufacturer == null)
            {
                return;
            }

            // Update modifiable fields
            existingManufacturer.Name = manufacturer.Name;

            await _dbcontext.SaveChangesAsync();
        }
    }
}