using MDMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Repository
{
    public class ConsumerRepository : IConsumerRepository
    {
        private readonly MdmsDbContext _dbcontext;

        public ConsumerRepository(MdmsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<IEnumerable<Consumer>> GetAllAsync()
        {
            return await _dbcontext.Consumers
                .Include(c => c.Status)
                .Include(c => c.Meters)
                .ToListAsync();
        }

        public async Task<Consumer> GetByIdAsync(int consumerId)
        {
            return await _dbcontext.Consumers
                .Include(c => c.Status)
                .Include(c => c.Meters)
                .FirstOrDefaultAsync(c => c.ConsumerId == consumerId);
        }

        public async Task AddAsync(Consumer consumer)
        {
            await _dbcontext.Consumers.AddAsync(consumer);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Consumer consumer)
        {
            var existing = await _dbcontext.Consumers.FirstOrDefaultAsync(c => c.ConsumerId == consumer.ConsumerId);
            if (existing == null)
            {
                return;
            }

            existing.Name = consumer.Name;
            existing.Address = consumer.Address;
            existing.Phone = consumer.Phone;
            existing.Email = consumer.Email;
            existing.StatusId = consumer.StatusId;
            existing.UpdatedAt = consumer.UpdatedAt;
            existing.UpdatedBy = consumer.UpdatedBy;

            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int consumerId)
        {
            var deleting = await _dbcontext.Consumers.FirstOrDefaultAsync(c => c.ConsumerId == consumerId);
            if (deleting != null)
            {
                _dbcontext.Consumers.Remove(deleting);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}