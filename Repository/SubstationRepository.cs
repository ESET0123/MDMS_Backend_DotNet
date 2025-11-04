using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MDMS_Backend.Repository
{
    public class SubstationRepository : ISubstationRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public SubstationRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(Substation newSubstation)
        {
            await _dbcontext.Substations.AddAsync(newSubstation);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deleting = await _dbcontext.Substations.FirstOrDefaultAsync(n => n.SubstationId == id);

            if (deleting != null)
            {
                _dbcontext.Substations.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Substation>> GetAllAsync()
        {
            // Include the related Zone for a richer context
            return await _dbcontext.Substations.Include(s => s.Zone).ToListAsync();
        }

        public async Task<Substation> GetByIdAsync(int id)
        {
            // Include the related Zone for a richer context
            return await _dbcontext.Substations.Include(s => s.Zone).FirstOrDefaultAsync(n => n.SubstationId == id);
        }

        public async Task UpdateAsync(Substation substation)
        {
            var existingSubstation = await _dbcontext.Substations.FirstOrDefaultAsync(n => n.SubstationId == substation.SubstationId);

            if (existingSubstation == null)
            {
                return;
            }

            // Update modifiable fields
            existingSubstation.SubstationName = substation.SubstationName;
            existingSubstation.ZoneId = substation.ZoneId;

            await _dbcontext.SaveChangesAsync();
        }
    }
    public class SubstationDTO
    {
        public int SubstationId { get; set; } // Used for updates

        [Required]
        public string SubstationName { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ZoneId must be a positive integer.")]
        public int ZoneId { get; set; }
    }
}
