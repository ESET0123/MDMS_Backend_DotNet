using MDMS_Backend.Models;
using System.Reflection;

namespace MDMS_Backend.Repository
{
    public interface ITodRuleRepository
    {
        Task<IEnumerable<TodRule>> GetAllAsync();
        Task<TodRule> GetByIdAsync(int id);
        Task<IEnumerable<TodRule>> GetActiveRulesAsync();
        Task<IEnumerable<TodRule>> GetRulesByTariffAsync(int tariffId);
        Task AddAsync(TodRule todRule);
        Task UpdateAsync(TodRule todRule);
        Task DeleteAsync(int id);
    }
}
