using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyBillController : ControllerBase
    {
        private readonly IMonthlyBillRepository _billRepo;
        private readonly IDailyMeterReadingRepository _readingRepo;

        public MonthlyBillController(IMonthlyBillRepository billRepo, IDailyMeterReadingRepository readingRepo)
        {
            _billRepo = billRepo;
            _readingRepo = readingRepo;
        }

        [HttpGet("AllBills")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetAllBills()
        {
            var bills = await _billRepo.GetAllAsync();

            var dtos = bills.Select(b => new MonthlyBillDetailDTO
            {
                BillId = b.BillId,
                MeterId = b.MeterId,
                //MeterNumber = b.Meter.MeterId ,
                ConsumerId = b.ConsumerId,
                ConsumerName = b.Consumer?.Name ?? "N/A",
                BillingMonth = b.BillingMonth,
                BillingYear = b.BillingYear,
                TotalConsumptionKwh = b.TotalConsumptionKwh,
                BaseAmount = b.BaseAmount,
                TotalSurgeCharges = b.TotalSurgeCharges,
                TotalDiscounts = b.TotalDiscounts,
                NetAmount = b.NetAmount,
                TaxAmount = b.TaxAmount,
                TotalAmount = b.TotalAmount,
                BillStatus = b.BillStatus,
                GeneratedAt = b.GeneratedAt
            });

            return Ok(dtos);
        }

        [HttpPost("Generate")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GenerateBill([FromBody] GenerateBillDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Get all readings for the period
            var readings = await _readingRepo.GetByMeterAndDateRangeAsync(
                model.MeterId,
                new DateOnly(model.Year, model.Month, 1),
                new DateOnly(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)));

            if (!readings.Any())
            {
                return BadRequest("No readings found for the specified period");
            }

            // Calculate totals
            var totalConsumption = readings.Sum(r => r.ConsumptionKwh);
            var baseAmount = readings.Sum(r => r.ConsumptionKwh * r.BaseRate);
            var totalSurgeCharges = readings.Sum(r => r.ConsumptionKwh * r.BaseRate * (r.SurgeChargePercent / 100));
            var totalDiscounts = readings.Sum(r => r.ConsumptionKwh * r.BaseRate * (r.DiscountPercent / 100));
            var netAmount = baseAmount + totalSurgeCharges - totalDiscounts;
            var taxAmount = netAmount * (model.TaxRate / 100);
            var totalAmount = netAmount + taxAmount;

            var bill = new MonthlyBill
            {
                MeterId = model.MeterId,
                ConsumerId = model.ConsumerId,
                BillingMonth = model.Month,
                BillingYear = model.Year,
                BillStartDate = new DateOnly(model.Year, model.Month, 1),
                BillEndDate = new DateOnly(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)),
                TotalConsumptionKwh = totalConsumption,
                BaseAmount = baseAmount,
                TotalSurgeCharges = totalSurgeCharges,
                TotalDiscounts = totalDiscounts,
                NetAmount = netAmount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                BillStatus = "Pending",
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = model.GeneratedBy
            };

            await _billRepo.AddAsync(bill);
            return CreatedAtAction(nameof(GetBillById), new { id = bill.BillId }, null);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(MonthlyBillDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MonthlyBillDetailDTO>> GetBillById(int id)
        {
            var bill = await _billRepo.GetByIdAsync(id);
            if (bill == null) return NotFound();

            var dto = new MonthlyBillDetailDTO
            {
                BillId = bill.BillId,
                MeterId = bill.MeterId,
                //MeterNumber = bill.Meter.MeterId ,
                ConsumerId = bill.ConsumerId,
                ConsumerName = bill.Consumer?.Name ?? "N/A",
                BillingMonth = bill.BillingMonth,
                BillingYear = bill.BillingYear,
                TotalConsumptionKwh = bill.TotalConsumptionKwh,
                BaseAmount = bill.BaseAmount,
                TotalSurgeCharges = bill.TotalSurgeCharges,
                TotalDiscounts = bill.TotalDiscounts,
                NetAmount = bill.NetAmount,
                TaxAmount = bill.TaxAmount,
                TotalAmount = bill.TotalAmount,
                BillStatus = bill.BillStatus,
                GeneratedAt = bill.GeneratedAt
            };

            return Ok(dto);
        }
    }

    public class GenerateBillDTO
    {
        [Required]
        public int MeterId { get; set; }

        [Required]
        public int ConsumerId { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal TaxRate { get; set; }

        [Required]
        public string GeneratedBy { get; set; } = null!;
    }

    public class MonthlyBillDetailDTO
    {
        public int BillId { get; set; }
        public int MeterId { get; set; }
        //public int MeterNumber { get; set; } = 0;
        public int ConsumerId { get; set; }
        public string ConsumerName { get; set; } = null!;
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public decimal TotalConsumptionKwh { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal TotalSurgeCharges { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillStatus { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
    }
}
