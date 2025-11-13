using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class TodRuleController : ControllerBase
    {
        private readonly ITodRuleRepository _todRuleRepo;

        public TodRuleController(ITodRuleRepository todRuleRepo)
        {
            _todRuleRepo = todRuleRepo;
        }

        [HttpGet("AllRules")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TodRuleDetailDTO>))]
        public async Task<ActionResult<IEnumerable<TodRuleDetailDTO>>> GetAllRules()
        {
            var rules = await _todRuleRepo.GetAllAsync();

            var dtos = rules.Select(r => new TodRuleDetailDTO
            {
                TodRuleId = r.TodRuleId,
                RuleName = r.RuleName,
                StartTime = r.StartTime.ToString(@"hh\:mm"),
                EndTime = r.EndTime.ToString(@"hh\:mm"),
                TariffId = r.TariffId,
                TariffName = r.Tariff?.Name ?? "N/A",
                SurgeChargePercent = r.SurgeChargePercent,
                DiscountPercent = r.DiscountPercent,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                CreatedBy = r.CreatedBy
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TodRuleDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TodRuleDetailDTO>> GetRuleById(int id)
        {
            var rule = await _todRuleRepo.GetByIdAsync(id);
            if (rule == null) return NotFound();

            var dto = new TodRuleDetailDTO
            {
                TodRuleId = rule.TodRuleId,
                RuleName = rule.RuleName,
                StartTime = rule.StartTime.ToString(@"hh\:mm"),
                EndTime = rule.EndTime.ToString(@"hh\:mm"),
                TariffId = rule.TariffId,
                TariffName = rule.Tariff?.Name ?? "N/A",
                SurgeChargePercent = rule.SurgeChargePercent,
                DiscountPercent = rule.DiscountPercent,
                IsActive = rule.IsActive,
                CreatedAt = rule.CreatedAt,
                CreatedBy = rule.CreatedBy
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateRule([FromBody] TodRuleDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newRule = new TodRule
            {
                RuleName = model.RuleName,
                StartTime = TimeOnly.Parse(model.StartTime),
                EndTime = TimeOnly.Parse(model.EndTime),
                TariffId = model.TariffId,
                SurgeChargePercent = model.SurgeChargePercent,
                DiscountPercent = model.DiscountPercent,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = model.CreatedBy
            };

            await _todRuleRepo.AddAsync(newRule);
            return CreatedAtAction(nameof(GetRuleById), new { id = newRule.TodRuleId }, null);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateRule([FromBody] TodRuleDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _todRuleRepo.GetByIdAsync(model.TodRuleId);
            if (existing == null) return NotFound();

            existing.RuleName = model.RuleName;
            existing.StartTime = TimeOnly.Parse(model.StartTime);
            existing.EndTime = TimeOnly.Parse(model.EndTime);
            existing.TariffId = model.TariffId;
            existing.SurgeChargePercent = model.SurgeChargePercent;
            existing.DiscountPercent = model.DiscountPercent;
            existing.IsActive = model.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = model.UpdatedBy;

            await _todRuleRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteRule(int id)
        {
            var existing = await _todRuleRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _todRuleRepo.DeleteAsync(id);
            return NoContent();
        }
    }

    public class TodRuleDTO
    {
        public int TodRuleId { get; set; }

        [Required]
        [StringLength(100)]
        public string RuleName { get; set; } = null!;

        [Required]
        public string StartTime { get; set; } = null!;

        [Required]
        public string EndTime { get; set; } = null!;

        [Required]
        public int TariffId { get; set; }

        [Range(0, 200)]
        public decimal SurgeChargePercent { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public string CreatedBy { get; set; } = null!;

        public string? UpdatedBy { get; set; }
    }

    public class TodRuleDetailDTO
    {
        public int TodRuleId { get; set; }
        public string RuleName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public int TariffId { get; set; }
        public string TariffName { get; set; } = null!;
        public decimal SurgeChargePercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
    }

    
}