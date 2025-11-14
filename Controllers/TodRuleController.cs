using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/TodRule/AllRules
        [HttpGet("AllRules")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TodRuleDTO>))]
        public async Task<ActionResult<IEnumerable<TodRuleDTO>>> GetAllRules()
        {
            var rules = await _todRuleRepo.GetAllAsync();
            var dtos = rules.Select(r => new TodRuleDTO
            {
                TodRuleId = r.TodRuleId,
                TariffId = r.TariffId,
                TariffName = r.Tariff?.Name ?? "Unknown",
                RuleName = r.RuleName,
                StartTime = r.StartTime.ToString("HH:mm"),
                EndTime = r.EndTime.ToString("HH:mm"),
                SurgeChargePercent = r.SurgeChargePercent,
                DiscountPercent = r.DiscountPercent,
                IsActive = r.IsActive
            });
            return Ok(dtos);
        }

        // GET: api/TodRule/ByTariff/1
        [HttpGet("ByTariff/{tariffId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TodRuleDTO>))]
        public async Task<ActionResult<IEnumerable<TodRuleDTO>>> GetRulesByTariff(int tariffId)
        {
            var rules = await _todRuleRepo.GetRulesByTariffAsync(tariffId);
            var dtos = rules.Select(r => new TodRuleDTO
            {
                TodRuleId = r.TodRuleId,
                TariffId = r.TariffId,
                TariffName = r.Tariff?.Name ?? "Unknown",
                RuleName = r.RuleName,
                StartTime = r.StartTime.ToString("HH:mm"),
                EndTime = r.EndTime.ToString("HH:mm"),
                SurgeChargePercent = r.SurgeChargePercent,
                DiscountPercent = r.DiscountPercent,
                IsActive = r.IsActive
            });
            return Ok(dtos);
        }

        // GET: api/TodRule/ActiveByTariff/1
        [HttpGet("ActiveByTariff/{tariffId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TodRuleDTO>))]
        public async Task<ActionResult<IEnumerable<TodRuleDTO>>> GetActiveRulesByTariff(int tariffId)
        {
            var rules = await _todRuleRepo.GetRulesByTariffAsync(tariffId);
            var activeRules = rules.Where(r => r.IsActive);

            var dtos = activeRules.Select(r => new TodRuleDTO
            {
                TodRuleId = r.TodRuleId,
                TariffId = r.TariffId,
                TariffName = r.Tariff?.Name ?? "Unknown",
                RuleName = r.RuleName,
                StartTime = r.StartTime.ToString("HH:mm"),
                EndTime = r.EndTime.ToString("HH:mm"),
                SurgeChargePercent = r.SurgeChargePercent,
                DiscountPercent = r.DiscountPercent,
                IsActive = r.IsActive
            }).OrderBy(r => r.StartTime);

            return Ok(dtos);
        }

        // GET: api/TodRule/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TodRuleDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TodRuleDTO>> GetById(int id)
        {
            var rule = await _todRuleRepo.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            var dto = new TodRuleDTO
            {
                TodRuleId = rule.TodRuleId,
                TariffId = rule.TariffId,
                TariffName = rule.Tariff?.Name ?? "Unknown",
                RuleName = rule.RuleName,
                StartTime = rule.StartTime.ToString("HH:mm"),
                EndTime = rule.EndTime.ToString("HH:mm"),
                SurgeChargePercent = rule.SurgeChargePercent,
                DiscountPercent = rule.DiscountPercent,
                IsActive = rule.IsActive
            };

            return Ok(dto);
        }

        // POST: api/TodRule/Create
        [HttpPost("Create")]
        [ProducesResponseType(201, Type = typeof(TodRuleDTO))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TodRuleDTO>> Create([FromBody] CreateTodRuleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate time format and logic
            if (!TimeOnly.TryParse(request.StartTime, out TimeOnly startTime) ||
                !TimeOnly.TryParse(request.EndTime, out TimeOnly endTime))
            {
                return BadRequest("Invalid time format. Use HH:mm format (24-hour).");
            }

            if (startTime >= endTime)
            {
                return BadRequest("Start time must be before end time.");
            }

            // Check for overlapping rules for the same tariff
            var existingRules = await _todRuleRepo.GetRulesByTariffAsync(request.TariffId);
            var hasOverlap = existingRules.Any(r =>
                r.IsActive &&
                IsTimeOverlap(startTime, endTime, r.StartTime, r.EndTime));

            if (hasOverlap)
            {
                return BadRequest("Time overlap detected with existing active rules for this tariff.");
            }

            var todRule = new TodRule
            {
                TariffId = request.TariffId,
                RuleName = request.RuleName,
                StartTime = startTime,
                EndTime = endTime,
                SurgeChargePercent = request.SurgeChargePercent,
                DiscountPercent = request.DiscountPercent,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy ?? "System",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = request.UpdatedBy ?? "System"
            };

            await _todRuleRepo.AddAsync(todRule);

            // Reload with tariff to get the name
            var createdRule = await _todRuleRepo.GetByIdAsync(todRule.TodRuleId);

            var dto = new TodRuleDTO
            {
                TodRuleId = createdRule.TodRuleId,
                TariffId = createdRule.TariffId,
                TariffName = createdRule.Tariff?.Name ?? "Unknown",
                RuleName = createdRule.RuleName,
                StartTime = createdRule.StartTime.ToString("HH:mm"),
                EndTime = createdRule.EndTime.ToString("HH:mm"),
                SurgeChargePercent = createdRule.SurgeChargePercent,
                DiscountPercent = createdRule.DiscountPercent,
                IsActive = createdRule.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.TodRuleId }, dto);
        }

        // PUT: api/TodRule/Update
        [HttpPut("Update")]
        [ProducesResponseType(200, Type = typeof(TodRuleDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TodRuleDTO>> UpdateWithIdInBody([FromBody] UpdateTodRuleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRule = await _todRuleRepo.GetByIdAsync(request.TodRuleId);
            if (existingRule == null)
                return NotFound();

            // Validate time format and logic
            if (!TimeOnly.TryParse(request.StartTime, out TimeOnly startTime) ||
                !TimeOnly.TryParse(request.EndTime, out TimeOnly endTime))
            {
                return BadRequest("Invalid time format. Use HH:mm format (24-hour).");
            }

            if (startTime >= endTime)
            {
                return BadRequest("Start time must be before end time.");
            }

            // Check for overlapping rules for the same tariff (excluding current rule)
            var existingRules = await _todRuleRepo.GetRulesByTariffAsync(request.TariffId);
            var hasOverlap = existingRules.Any(r =>
                r.IsActive &&
                r.TodRuleId != request.TodRuleId &&
                IsTimeOverlap(startTime, endTime, r.StartTime, r.EndTime));

            if (hasOverlap)
            {
                return BadRequest("Time overlap detected with existing active rules for this tariff.");
            }

            existingRule.RuleName = request.RuleName;
            existingRule.StartTime = startTime;
            existingRule.EndTime = endTime;
            existingRule.TariffId = request.TariffId;
            existingRule.SurgeChargePercent = request.SurgeChargePercent;
            existingRule.DiscountPercent = request.DiscountPercent;
            existingRule.IsActive = request.IsActive;
            existingRule.UpdatedAt = DateTime.UtcNow;
            existingRule.UpdatedBy = request.UpdatedBy ?? "System";

            await _todRuleRepo.UpdateAsync(existingRule);

            // Reload with tariff to get the name
            var updatedRule = await _todRuleRepo.GetByIdAsync(request.TodRuleId);

            var dto = new TodRuleDTO
            {
                TodRuleId = updatedRule.TodRuleId,
                TariffId = updatedRule.TariffId,
                TariffName = updatedRule.Tariff?.Name ?? "Unknown",
                RuleName = updatedRule.RuleName,
                StartTime = updatedRule.StartTime.ToString("HH:mm"),
                EndTime = updatedRule.EndTime.ToString("HH:mm"),
                SurgeChargePercent = updatedRule.SurgeChargePercent,
                DiscountPercent = updatedRule.DiscountPercent,
                IsActive = updatedRule.IsActive
            };

            return Ok(dto);
        }

        // DELETE: api/TodRule/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingRule = await _todRuleRepo.GetByIdAsync(id);
            if (existingRule == null)
                return NotFound();

            await _todRuleRepo.DeleteAsync(id);
            return NoContent();
        }

        // PATCH: api/TodRule/5/ToggleStatus
        [HttpPatch("{id}/ToggleStatus")]
        [ProducesResponseType(200, Type = typeof(TodRuleDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TodRuleDTO>> ToggleStatus(int id)
        {
            var existingRule = await _todRuleRepo.GetByIdAsync(id);
            if (existingRule == null)
                return NotFound();

            existingRule.IsActive = !existingRule.IsActive;
            existingRule.UpdatedAt = DateTime.UtcNow;
            existingRule.UpdatedBy = "System";

            await _todRuleRepo.UpdateAsync(existingRule);

            var dto = new TodRuleDTO
            {
                TodRuleId = existingRule.TodRuleId,
                TariffId = existingRule.TariffId,
                TariffName = existingRule.Tariff?.Name ?? "Unknown",
                RuleName = existingRule.RuleName,
                StartTime = existingRule.StartTime.ToString("HH:mm"),
                EndTime = existingRule.EndTime.ToString("HH:mm"),
                SurgeChargePercent = existingRule.SurgeChargePercent,
                DiscountPercent = existingRule.DiscountPercent,
                IsActive = existingRule.IsActive
            };

            return Ok(dto);
        }

        // Helper method to check for time overlaps
        private bool IsTimeOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
        {
            return start1 < end2 && end1 > start2;
        }
    }

    public class TodRuleDTO
    {
        public int TodRuleId { get; set; }
        public int TariffId { get; set; }
        public string TariffName { get; set; } = null!;
        public string RuleName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public decimal SurgeChargePercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTodRuleRequest
    {
        public int TariffId { get; set; }
        public string RuleName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public decimal SurgeChargePercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class UpdateTodRuleRequest
    {
        public int TodRuleId { get; set; }
        public int TariffId { get; set; }
        public string RuleName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public decimal SurgeChargePercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
    }
}