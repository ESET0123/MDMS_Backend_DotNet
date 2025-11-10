using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyMeterReadingController : ControllerBase
    {
        private readonly IDailyMeterReadingRepository _readingRepo;
        private readonly ITodRuleRepository _todRuleRepo;
        private readonly IMeterRepository _meterRepo;

        public DailyMeterReadingController(
            IDailyMeterReadingRepository readingRepo,
            ITodRuleRepository todRuleRepo,
            IMeterRepository meterRepo)
        {
            _readingRepo = readingRepo;
            _todRuleRepo = todRuleRepo;
            _meterRepo = meterRepo;
        }

        [HttpGet("ByMeter/{meterId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DailyReadingDetailDTO>))]
        public async Task<ActionResult<IEnumerable<DailyReadingDetailDTO>>> GetByMeter(int meterId)
        {
            var readings = await _readingRepo.GetByMeterAsync(meterId);

            var dtos = readings.Select(r => new DailyReadingDetailDTO
            {
                ReadingId = r.ReadingId,
                MeterId = r.MeterId,
                ReadingDate = r.ReadingDate,
                TodRuleId = r.TodRuleId,
                TodRuleName = r.TodRule?.RuleName ?? "N/A",
                StartTime = r.StartTime.ToString(@"hh\:mm"),
                EndTime = r.EndTime.ToString(@"hh\:mm"),
                PreviousReading = r.PreviousReading,
                CurrentReading = r.CurrentReading,
                ConsumptionKwh = r.ConsumptionKwh,
                BaseRate = r.BaseRate,
                SurgeChargePercent = r.SurgeChargePercent,
                DiscountPercent = r.DiscountPercent,
                EffectiveRate = r.EffectiveRate,
                Amount = r.Amount
            });

            return Ok(dtos);
        }

        [HttpGet("ByDateRange")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DailyReadingDetailDTO>))]
        public async Task<ActionResult<IEnumerable<DailyReadingDetailDTO>>> GetByDateRange(
            [FromQuery] int meterId,
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            var readings = await _readingRepo.GetByMeterAndDateRangeAsync(meterId, startDate, endDate);

            var dtos = readings.Select(r => new DailyReadingDetailDTO
            {
                ReadingId = r.ReadingId,
                MeterId = r.MeterId,
                ReadingDate = r.ReadingDate,
                TodRuleId = r.TodRuleId,
                TodRuleName = r.TodRule?.RuleName ?? "N/A",
                StartTime = r.StartTime.ToString(@"hh\:mm"),
                EndTime = r.EndTime.ToString(@"hh\:mm"),
                PreviousReading = r.PreviousReading,
                CurrentReading = r.CurrentReading,
                ConsumptionKwh = r.ConsumptionKwh,
                BaseRate = r.BaseRate,
                SurgeChargePercent = r.SurgeChargePercent,
                DiscountPercent = r.DiscountPercent,
                EffectiveRate = r.EffectiveRate,
                Amount = r.Amount
            });

            return Ok(dtos);
        }

        [HttpPost("CreateBulk")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateBulkReadings([FromBody] BulkReadingDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate meter exists
            var meter = await _meterRepo.GetByIdAsync(model.MeterId);
            if (meter == null)
                return BadRequest(new { error = "Meter not found" });

            var readings = new List<DailyMeterReading>();
            var errors = new List<string>();

            foreach (var reading in model.Readings)
            {
                // Validate TOD rule exists
                var todRule = await _todRuleRepo.GetByIdAsync(reading.TodRuleId);
                if (todRule == null)
                {
                    errors.Add($"TOD Rule ID {reading.TodRuleId} not found");
                    continue;
                }

                // Validate readings
                if (reading.CurrentReading < reading.PreviousReading)
                {
                    errors.Add($"Current reading ({reading.CurrentReading}) cannot be less than previous reading ({reading.PreviousReading})");
                    continue;
                }

                var consumption = reading.CurrentReading - reading.PreviousReading;
                var baseRate = reading.BaseRate;
                var surgeAmount = baseRate * (todRule.SurgeChargePercent / 100);
                var discountAmount = baseRate * (todRule.DiscountPercent / 100);
                var effectiveRate = baseRate + surgeAmount - discountAmount;
                var amount = consumption * effectiveRate;

                var dailyReading = new DailyMeterReading
                {
                    MeterId = model.MeterId,
                    ReadingDate = model.ReadingDate,
                    TodRuleId = reading.TodRuleId,
                    StartTime = TimeOnly.Parse(reading.StartTime),
                    EndTime = TimeOnly.Parse(reading.EndTime),
                    PreviousReading = reading.PreviousReading,
                    CurrentReading = reading.CurrentReading,
                    ConsumptionKwh = consumption,
                    BaseRate = baseRate,
                    SurgeChargePercent = todRule.SurgeChargePercent,
                    DiscountPercent = todRule.DiscountPercent,
                    EffectiveRate = effectiveRate,
                    Amount = amount,
                    RecordedBy = model.RecordedBy,
                    CreatedAt = DateTime.UtcNow
                };

                readings.Add(dailyReading);
            }

            if (errors.Any())
            {
                return BadRequest(new { errors = errors });
            }

            if (readings.Count == 0)
            {
                return BadRequest(new { error = "No valid readings to save" });
            }

            await _readingRepo.AddRangeAsync(readings);

            // Update meter's latest reading
            var latestReading = readings.Max(r => r.CurrentReading);
            meter.LatestReading = latestReading;
            await _meterRepo.UpdateAsync(meter);

            return Ok(new
            {
                message = "Readings created successfully",
                count = readings.Count,
                totalConsumption = readings.Sum(r => r.ConsumptionKwh),
                totalAmount = readings.Sum(r => r.Amount)
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteReading(int id)
        {
            var existing = await _readingRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _readingRepo.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("ValidateReading")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> ValidateReading(
            [FromQuery] int meterId,
            [FromQuery] decimal currentReading)
        {
            var meter = await _meterRepo.GetByIdAsync(meterId);
            if (meter == null)
                return NotFound(new { error = "Meter not found" });

            var isValid = currentReading >= meter.LatestReading;

            return Ok(new
            {
                isValid = isValid,
                latestReading = meter.LatestReading,
                message = isValid
                    ? "Reading is valid"
                    : $"Current reading must be >= {meter.LatestReading}"
            });
        }
    }

    public class BulkReadingDTO
    {
        [Required]
        public int MeterId { get; set; }

        [Required]
        public DateOnly ReadingDate { get; set; }

        [Required]
        [StringLength(100)]
        public string RecordedBy { get; set; } = null!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one reading is required")]
        public List<SingleReadingDTO> Readings { get; set; } = new();
    }

    public class SingleReadingDTO
    {
        [Required]
        public int TodRuleId { get; set; }

        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format")]
        public string StartTime { get; set; } = null!;

        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format")]
        public string EndTime { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Previous reading must be non-negative")]
        public decimal PreviousReading { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Current reading must be non-negative")]
        public decimal CurrentReading { get; set; }

        [Required]
        [Range(0.0001, 999.9999, ErrorMessage = "Base rate must be between 0.0001 and 999.9999")]
        public decimal BaseRate { get; set; }
    }

    public class DailyReadingDetailDTO
    {
        public int ReadingId { get; set; }
        public int MeterId { get; set; }
        public DateOnly ReadingDate { get; set; }
        public int TodRuleId { get; set; }
        public string TodRuleName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal ConsumptionKwh { get; set; }
        public decimal BaseRate { get; set; }
        public decimal SurgeChargePercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal EffectiveRate { get; set; }
        public decimal Amount { get; set; }
    }
}