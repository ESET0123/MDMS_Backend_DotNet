////using MDMS_Backend.Models;
////using MDMS_Backend.Repository;
////using Microsoft.AspNetCore.Mvc;
////using System.ComponentModel.DataAnnotations;

////namespace MDMS_Backend.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class MonthlyBillController : ControllerBase
////    {
////        private readonly IMonthlyBillRepository _billRepo;
////        private readonly IDailyMeterReadingRepository _readingRepo;

////        public MonthlyBillController(IMonthlyBillRepository billRepo, IDailyMeterReadingRepository readingRepo)
////        {
////            _billRepo = billRepo;
////            _readingRepo = readingRepo;
////        }

////        [HttpGet("AllBills")]
////        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
////        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetAllBills()
////        {
////            var bills = await _billRepo.GetAllAsync();

////            var dtos = bills.Select(b => new MonthlyBillDetailDTO
////            {
////                BillId = b.BillId,
////                MeterId = b.MeterId,
////                //MeterNumber = b.Meter.MeterId ,
////                ConsumerId = b.ConsumerId,
////                ConsumerName = b.Consumer?.Name ?? "N/A",
////                BillingMonth = b.BillingMonth,
////                BillingYear = b.BillingYear,
////                TotalConsumptionKwh = b.TotalConsumptionKwh,
////                BaseAmount = b.BaseAmount,
////                TotalSurgeCharges = b.TotalSurgeCharges,
////                TotalDiscounts = b.TotalDiscounts,
////                NetAmount = b.NetAmount,
////                TaxAmount = b.TaxAmount,
////                TotalAmount = b.TotalAmount,
////                BillStatus = b.BillStatus,
////                GeneratedAt = b.GeneratedAt
////            });

////            return Ok(dtos);
////        }

////        [HttpPost("Generate")]
////        [ProducesResponseType(201)]
////        [ProducesResponseType(400)]
////        public async Task<ActionResult> GenerateBill([FromBody] GenerateBillDTO model)
////        {
////            if (!ModelState.IsValid) return BadRequest(ModelState);

////            // Get all readings for the period
////            var readings = await _readingRepo.GetByMeterAndDateRangeAsync(
////                model.MeterId,
////                new DateOnly(model.Year, model.Month, 1),
////                new DateOnly(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)));

////            if (!readings.Any())
////            {
////                return BadRequest("No readings found for the specified period");
////            }

////            // Calculate totals
////            var totalConsumption = readings.Sum(r => r.ConsumptionKwh);
////            var baseAmount = readings.Sum(r => r.ConsumptionKwh * r.BaseRate);
////            var totalSurgeCharges = readings.Sum(r => r.ConsumptionKwh * r.BaseRate * (r.SurgeChargePercent / 100));
////            var totalDiscounts = readings.Sum(r => r.ConsumptionKwh * r.BaseRate * (r.DiscountPercent / 100));
////            var netAmount = baseAmount + totalSurgeCharges - totalDiscounts;
////            var taxAmount = netAmount * (model.TaxRate / 100);
////            var totalAmount = netAmount + taxAmount;

////            var bill = new MonthlyBill
////            {
////                MeterId = model.MeterId,
////                ConsumerId = model.ConsumerId,
////                BillingMonth = model.Month,
////                BillingYear = model.Year,
////                BillStartDate = new DateOnly(model.Year, model.Month, 1),
////                BillEndDate = new DateOnly(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)),
////                TotalConsumptionKwh = totalConsumption,
////                BaseAmount = baseAmount,
////                TotalSurgeCharges = totalSurgeCharges,
////                TotalDiscounts = totalDiscounts,
////                NetAmount = netAmount,
////                TaxAmount = taxAmount,
////                TotalAmount = totalAmount,
////                BillStatus = "Pending",
////                GeneratedAt = DateTime.UtcNow,
////                GeneratedBy = model.GeneratedBy
////            };

////            await _billRepo.AddAsync(bill);
////            return CreatedAtAction(nameof(GetBillById), new { id = bill.BillId }, null);
////        }

////        [HttpGet("{id}")]
////        [ProducesResponseType(200, Type = typeof(MonthlyBillDetailDTO))]
////        [ProducesResponseType(404)]
////        public async Task<ActionResult<MonthlyBillDetailDTO>> GetBillById(int id)
////        {
////            var bill = await _billRepo.GetByIdAsync(id);
////            if (bill == null) return NotFound();

////            var dto = new MonthlyBillDetailDTO
////            {
////                BillId = bill.BillId,
////                MeterId = bill.MeterId,
////                //MeterNumber = bill.Meter.MeterId ,
////                ConsumerId = bill.ConsumerId,
////                ConsumerName = bill.Consumer?.Name ?? "N/A",
////                BillingMonth = bill.BillingMonth,
////                BillingYear = bill.BillingYear,
////                TotalConsumptionKwh = bill.TotalConsumptionKwh,
////                BaseAmount = bill.BaseAmount,
////                TotalSurgeCharges = bill.TotalSurgeCharges,
////                TotalDiscounts = bill.TotalDiscounts,
////                NetAmount = bill.NetAmount,
////                TaxAmount = bill.TaxAmount,
////                TotalAmount = bill.TotalAmount,
////                BillStatus = bill.BillStatus,
////                GeneratedAt = bill.GeneratedAt
////            };

////            return Ok(dto);
////        }
////    }

////    public class GenerateBillDTO
////    {
////        [Required]
////        public int MeterId { get; set; }

////        [Required]
////        public int ConsumerId { get; set; }

////        [Required]
////        [Range(1, 12)]
////        public int Month { get; set; }

////        [Required]
////        public int Year { get; set; }

////        [Required]
////        [Range(0, 100)]
////        public decimal TaxRate { get; set; }

////        [Required]
////        public string GeneratedBy { get; set; } = null!;
////    }

////    public class MonthlyBillDetailDTO
////    {
////        public int BillId { get; set; }
////        public int MeterId { get; set; }
////        //public int MeterNumber { get; set; } = 0;
////        public int ConsumerId { get; set; }
////        public string ConsumerName { get; set; } = null!;
////        public int BillingMonth { get; set; }
////        public int BillingYear { get; set; }
////        public decimal TotalConsumptionKwh { get; set; }
////        public decimal BaseAmount { get; set; }
////        public decimal TotalSurgeCharges { get; set; }
////        public decimal TotalDiscounts { get; set; }
////        public decimal NetAmount { get; set; }
////        public decimal TaxAmount { get; set; }
////        public decimal TotalAmount { get; set; }
////        public string BillStatus { get; set; } = null!;
////        public DateTime GeneratedAt { get; set; }
////    }
////}
//using MDMS_Backend.Models;
//using MDMS_Backend.Repository;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;

//namespace MDMS_Backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MonthlyBillController : ControllerBase
//    {
//        private readonly IMonthlyBillRepository _billRepo;
//        private readonly IDailyMeterReadingRepository _readingRepo;
//        private readonly IMeterRepository _meterRepo;

//        public MonthlyBillController(
//            IMonthlyBillRepository billRepo,
//            IDailyMeterReadingRepository readingRepo,
//            IMeterRepository meterRepo)
//        {
//            _billRepo = billRepo;
//            _readingRepo = readingRepo;
//            _meterRepo = meterRepo;
//        }

//        [HttpGet("AllBills")]
//        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
//        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetAllBills()
//        {
//            var bills = await _billRepo.GetAllAsync();

//            var dtos = bills.Select(b => new MonthlyBillDetailDTO
//            {
//                BillId = b.BillId,
//                MeterId = b.MeterId,
//                ConsumerId = b.ConsumerId,
//                ConsumerName = b.Consumer?.Name ?? "N/A",
//                BillingMonth = b.BillingMonth,
//                BillingYear = b.BillingYear,
//                TotalConsumptionKwh = b.TotalConsumptionKwh,
//                BaseAmount = b.BaseAmount,
//                TotalSurgeCharges = b.TotalSurgeCharges,
//                TotalDiscounts = b.TotalDiscounts,
//                NetAmount = b.NetAmount,
//                TaxAmount = b.TaxAmount,
//                TotalAmount = b.TotalAmount,
//                BillStatus = b.BillStatus,
//                GeneratedAt = b.GeneratedAt
//            });

//            return Ok(dtos);
//        }

//        [HttpGet("ByMeter/{meterId}")]
//        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
//        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetByMeter(int meterId)
//        {
//            var meter = await _meterRepo.GetByIdAsync(meterId);
//            if (meter == null)
//                return NotFound(new { error = "Meter not found" });

//            var bills = await _billRepo.GetByConsumerAsync(meter.ConsumerId);

//            var dtos = bills.Where(b => b.MeterId == meterId).Select(b => new MonthlyBillDetailDTO
//            {
//                BillId = b.BillId,
//                MeterId = b.MeterId,
//                ConsumerId = b.ConsumerId,
//                ConsumerName = b.Consumer?.Name ?? "N/A",
//                BillingMonth = b.BillingMonth,
//                BillingYear = b.BillingYear,
//                TotalConsumptionKwh = b.TotalConsumptionKwh,
//                BaseAmount = b.BaseAmount,
//                TotalSurgeCharges = b.TotalSurgeCharges,
//                TotalDiscounts = b.TotalDiscounts,
//                NetAmount = b.NetAmount,
//                TaxAmount = b.TaxAmount,
//                TotalAmount = b.TotalAmount,
//                BillStatus = b.BillStatus,
//                GeneratedAt = b.GeneratedAt
//            });

//            return Ok(dtos);
//        }

//        [HttpPost("Generate")]
//        [ProducesResponseType(201)]
//        [ProducesResponseType(400)]
//        public async Task<ActionResult> GenerateBill([FromBody] GenerateBillDTO model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Validate meter exists
//            var meter = await _meterRepo.GetByIdAsync(model.MeterId);
//            if (meter == null)
//                return BadRequest(new { error = "Meter not found" });

//            // Check if bill already exists for this period
//            var existingBill = await _billRepo.GetByMeterAndMonthAsync(model.MeterId, model.Year, model.Month);
//            if (existingBill != null)
//            {
//                return BadRequest(new
//                {
//                    error = $"Bill already exists for Meter #{model.MeterId} for {GetMonthName(model.Month)} {model.Year}",
//                    existingBillId = existingBill.BillId
//                });
//            }

//            // Get all readings for the period
//            var startDate = new DateOnly(model.Year, model.Month, 1);
//            var endDate = new DateOnly(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month));

//            var readings = await _readingRepo.GetByMeterAndDateRangeAsync(model.MeterId, startDate, endDate);

//            if (!readings.Any())
//            {
//                return BadRequest(new
//                {
//                    error = $"No daily readings found for Meter #{model.MeterId} in {GetMonthName(model.Month)} {model.Year}. Please add daily readings first."
//                });
//            }

//            // Calculate totals from daily readings
//            var totalConsumption = readings.Sum(r => r.ConsumptionKwh);
//            var baseAmount = readings.Sum(r => r.ConsumptionKwh * r.BaseRate);

//            // Calculate surge charges and discounts
//            var totalSurgeCharges = readings.Sum(r =>
//                r.ConsumptionKwh * r.BaseRate * (r.SurgeChargePercent / 100));
//            var totalDiscounts = readings.Sum(r =>
//                r.ConsumptionKwh * r.BaseRate * (r.DiscountPercent / 100));

//            var netAmount = baseAmount + totalSurgeCharges - totalDiscounts;
//            var taxAmount = netAmount * (model.TaxRate / 100);
//            var totalAmount = netAmount + taxAmount;

//            var bill = new MonthlyBill
//            {
//                MeterId = model.MeterId,
//                ConsumerId = meter.ConsumerId,
//                BillingMonth = model.Month,
//                BillingYear = model.Year,
//                BillStartDate = startDate,
//                BillEndDate = endDate,
//                TotalConsumptionKwh = totalConsumption,
//                BaseAmount = baseAmount,
//                TotalSurgeCharges = totalSurgeCharges,
//                TotalDiscounts = totalDiscounts,
//                NetAmount = netAmount,
//                TaxAmount = taxAmount,
//                TotalAmount = totalAmount,
//                BillStatus = "Pending",
//                GeneratedAt = DateTime.UtcNow,
//                GeneratedBy = model.GeneratedBy
//            };

//            await _billRepo.AddAsync(bill);

//            return Ok(new
//            {
//                message = "Bill generated successfully",
//                billId = bill.BillId,
//                totalConsumption = totalConsumption,
//                totalAmount = totalAmount,
//                readingsCount = readings.Count()
//            });
//        }

//        [HttpGet("{id}")]
//        [ProducesResponseType(200, Type = typeof(MonthlyBillDetailDTO))]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult<MonthlyBillDetailDTO>> GetBillById(int id)
//        {
//            var bill = await _billRepo.GetByIdAsync(id);
//            if (bill == null)
//                return NotFound();

//            var dto = new MonthlyBillDetailDTO
//            {
//                BillId = bill.BillId,
//                MeterId = bill.MeterId,
//                ConsumerId = bill.ConsumerId,
//                ConsumerName = bill.Consumer?.Name ?? "N/A",
//                BillingMonth = bill.BillingMonth,
//                BillingYear = bill.BillingYear,
//                TotalConsumptionKwh = bill.TotalConsumptionKwh,
//                BaseAmount = bill.BaseAmount,
//                TotalSurgeCharges = bill.TotalSurgeCharges,
//                TotalDiscounts = bill.TotalDiscounts,
//                NetAmount = bill.NetAmount,
//                TaxAmount = bill.TaxAmount,
//                TotalAmount = bill.TotalAmount,
//                BillStatus = bill.BillStatus,
//                GeneratedAt = bill.GeneratedAt
//            };

//            return Ok(dto);
//        }

//        [HttpPut("{id}/Status")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult> UpdateBillStatus(int id, [FromBody] UpdateBillStatusDTO model)
//        {
//            var bill = await _billRepo.GetByIdAsync(id);
//            if (bill == null)
//                return NotFound();

//            bill.BillStatus = model.Status;
//            if (model.Status == "Paid")
//            {
//                bill.PaidDate = DateTime.UtcNow;
//            }

//            await _billRepo.UpdateAsync(bill);
//            return Ok(new { message = "Bill status updated successfully" });
//        }

//        [HttpDelete("{id}")]
//        [ProducesResponseType(204)]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult> DeleteBill(int id)
//        {
//            var bill = await _billRepo.GetByIdAsync(id);
//            if (bill == null)
//                return NotFound();

//            await _billRepo.DeleteAsync(id);
//            return NoContent();
//        }

//        [HttpGet("Summary/{meterId}")]
//        [ProducesResponseType(200)]
//        public async Task<ActionResult> GetMeterBillSummary(int meterId)
//        {
//            var meter = await _meterRepo.GetByIdAsync(meterId);
//            if (meter == null)
//                return NotFound(new { error = "Meter not found" });

//            var bills = await _billRepo.GetByConsumerAsync(meter.ConsumerId);
//            var meterBills = bills.Where(b => b.MeterId == meterId).ToList();

//            var summary = new
//            {
//                meterId = meterId,
//                totalBills = meterBills.Count,
//                pendingBills = meterBills.Count(b => b.BillStatus == "Pending"),
//                paidBills = meterBills.Count(b => b.BillStatus == "Paid"),
//                overdueBills = meterBills.Count(b => b.BillStatus == "Overdue"),
//                totalAmount = meterBills.Sum(b => b.TotalAmount),
//                pendingAmount = meterBills.Where(b => b.BillStatus == "Pending").Sum(b => b.TotalAmount),
//                totalConsumption = meterBills.Sum(b => b.TotalConsumptionKwh)
//            };

//            return Ok(summary);
//        }

//        private string GetMonthName(int month)
//        {
//            return new DateTime(2000, month, 1).ToString("MMMM");
//        }
//    }

//    public class GenerateBillDTO
//    {
//        [Required]
//        public int MeterId { get; set; }

//        [Required]
//        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
//        public int Month { get; set; }

//        [Required]
//        [Range(2000, 2100, ErrorMessage = "Invalid year")]
//        public int Year { get; set; }

//        [Required]
//        [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
//        public decimal TaxRate { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string GeneratedBy { get; set; } = null!;
//    }

//    public class UpdateBillStatusDTO
//    {
//        [Required]
//        [RegularExpression("^(Pending|Paid|Overdue)$", ErrorMessage = "Invalid status")]
//        public string Status { get; set; } = null!;
//    }

//    public class MonthlyBillDetailDTO
//    {
//        public int BillId { get; set; }
//        public int MeterId { get; set; }
//        public int ConsumerId { get; set; }
//        public string ConsumerName { get; set; } = null!;
//        public int BillingMonth { get; set; }
//        public int BillingYear { get; set; }
//        public decimal TotalConsumptionKwh { get; set; }
//        public decimal BaseAmount { get; set; }
//        public decimal TotalSurgeCharges { get; set; }
//        public decimal TotalDiscounts { get; set; }
//        public decimal NetAmount { get; set; }
//        public decimal TaxAmount { get; set; }
//        public decimal TotalAmount { get; set; }
//        public string BillStatus { get; set; } = null!;
//        public DateTime GeneratedAt { get; set; }
//    }
//}

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
        private readonly IDailyMeterReadingRepository _readingRepo;
        private readonly IMeterRepository _meterRepo;
        private readonly IConsumerRepository _consumerRepo;

        public MonthlyBillController(
            IDailyMeterReadingRepository readingRepo,
            IMeterRepository meterRepo,
            IConsumerRepository consumerRepo)
        {
            _readingRepo = readingRepo;
            _meterRepo = meterRepo;
            _consumerRepo = consumerRepo;
        }

        [HttpGet("AllBills")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetAllBills(
            [FromQuery] int? year = null,
            [FromQuery] int? month = null)
        {
            try
            {
                // Get all daily readings
                var allReadings = await _readingRepo.GetAllAsync();

                if (!allReadings.Any())
                    return Ok(new List<MonthlyBillDetailDTO>());

                var groupedReadings = allReadings
                    .GroupBy(r => new
                    {
                        r.MeterId,
                        Year = r.ReadingDate.Year,
                        Month = r.ReadingDate.Month
                    })
                    .ToList();

                if (year.HasValue)
                    groupedReadings = groupedReadings.Where(g => g.Key.Year == year.Value).ToList();

                if (month.HasValue)
                    groupedReadings = groupedReadings.Where(g => g.Key.Month == month.Value).ToList();

                var bills = new List<MonthlyBillDetailDTO>();

                foreach (var group in groupedReadings)
                {
                    var readings = group.ToList();
                    var meter = await _meterRepo.GetByIdAsync(group.Key.MeterId);

                    if (meter == null) continue;

                    var consumer = await _consumerRepo.GetByIdAsync(meter.ConsumerId);

                    var totalConsumption = readings.Sum(r => r.ConsumptionKwh);
                    var baseAmount = readings.Sum(r => r.ConsumptionKwh * r.BaseRate);
                    var totalSurgeCharges = readings.Sum(r =>
                        r.ConsumptionKwh * r.BaseRate * (r.SurgeChargePercent / 100));
                    var totalDiscounts = readings.Sum(r =>
                        r.ConsumptionKwh * r.BaseRate * (r.DiscountPercent / 100));
                    var netAmount = baseAmount + totalSurgeCharges - totalDiscounts;
                    var taxAmount = netAmount * 0.085m;
                    var totalAmount = netAmount + taxAmount;

                    var billEndDate = new DateOnly(group.Key.Year, group.Key.Month,
                        DateTime.DaysInMonth(group.Key.Year, group.Key.Month));
                    var currentDate = DateOnly.FromDateTime(DateTime.Now);

                    string billStatus = "Pending";
                    if (currentDate > billEndDate.AddDays(30))
                        billStatus = "Overdue";
                    else if (currentDate <= billEndDate)
                        billStatus = "Pending";

                    bills.Add(new MonthlyBillDetailDTO
                    {
                        BillId = $"{group.Key.MeterId}-{group.Key.Year}-{group.Key.Month}",
                        MeterId = group.Key.MeterId,
                        ConsumerId = meter.ConsumerId,
                        ConsumerName = consumer?.Name ?? "N/A",
                        BillingMonth = group.Key.Month,
                        BillingYear = group.Key.Year,
                        TotalConsumptionKwh = totalConsumption,
                        BaseAmount = baseAmount,
                        TotalSurgeCharges = totalSurgeCharges,
                        TotalDiscounts = totalDiscounts,
                        NetAmount = netAmount,
                        TaxAmount = taxAmount,
                        TotalAmount = totalAmount,
                        BillStatus = billStatus,
                        ReadingsCount = readings.Count,
                        StartDate = readings.Min(r => r.ReadingDate),
                        EndDate = readings.Max(r => r.ReadingDate)
                    });
                }

                bills = bills.OrderByDescending(b => b.BillingYear)
                           .ThenByDescending(b => b.BillingMonth)
                           .ToList();

                return Ok(bills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to calculate bills", details = ex.Message });
            }
        }

        [HttpGet("ByMeter/{meterId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetByMeter(int meterId)
        {
            var meter = await _meterRepo.GetByIdAsync(meterId);
            if (meter == null)
                return NotFound(new { error = "Meter not found" });

            var readings = await _readingRepo.GetByMeterAsync(meterId);

            if (!readings.Any())
                return Ok(new List<MonthlyBillDetailDTO>());

            var groupedReadings = readings
                .GroupBy(r => new { Year = r.ReadingDate.Year, Month = r.ReadingDate.Month })
                .ToList();

            var bills = new List<MonthlyBillDetailDTO>();
            var consumer = await _consumerRepo.GetByIdAsync(meter.ConsumerId);

            foreach (var group in groupedReadings)
            {
                var monthReadings = group.ToList();

                var totalConsumption = monthReadings.Sum(r => r.ConsumptionKwh);
                var baseAmount = monthReadings.Sum(r => r.ConsumptionKwh * r.BaseRate);
                var totalSurgeCharges = monthReadings.Sum(r =>
                    r.ConsumptionKwh * r.BaseRate * (r.SurgeChargePercent / 100));
                var totalDiscounts = monthReadings.Sum(r =>
                    r.ConsumptionKwh * r.BaseRate * (r.DiscountPercent / 100));
                var netAmount = baseAmount + totalSurgeCharges - totalDiscounts;
                var taxAmount = netAmount * 0.085m;
                var totalAmount = netAmount + taxAmount;

                var billEndDate = new DateOnly(group.Key.Year, group.Key.Month,
                    DateTime.DaysInMonth(group.Key.Year, group.Key.Month));
                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                string billStatus = currentDate > billEndDate.AddDays(30) ? "Overdue" : "Pending";

                bills.Add(new MonthlyBillDetailDTO
                {
                    BillId = $"{meterId}-{group.Key.Year}-{group.Key.Month}",
                    MeterId = meterId,
                    ConsumerId = meter.ConsumerId,
                    ConsumerName = consumer?.Name ?? "N/A",
                    BillingMonth = group.Key.Month,
                    BillingYear = group.Key.Year,
                    TotalConsumptionKwh = totalConsumption,
                    BaseAmount = baseAmount,
                    TotalSurgeCharges = totalSurgeCharges,
                    TotalDiscounts = totalDiscounts,
                    NetAmount = netAmount,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    BillStatus = billStatus,
                    ReadingsCount = monthReadings.Count,
                    StartDate = monthReadings.Min(r => r.ReadingDate),
                    EndDate = monthReadings.Max(r => r.ReadingDate)
                });
            }

            bills = bills.OrderByDescending(b => b.BillingYear)
                       .ThenByDescending(b => b.BillingMonth)
                       .ToList();

            return Ok(bills);
        }

        [HttpGet("Details")]
        [ProducesResponseType(200, Type = typeof(MonthlyBillDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MonthlyBillDetailDTO>> GetBillDetails(
            [FromQuery] int meterId,
            [FromQuery] int year,
            [FromQuery] int month)
        {
            var meter = await _meterRepo.GetByIdAsync(meterId);
            if (meter == null)
                return NotFound(new { error = "Meter not found" });

            var startDate = new DateOnly(year, month, 1);
            var endDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var readings = await _readingRepo.GetByMeterAndDateRangeAsync(meterId, startDate, endDate);

            if (!readings.Any())
                return NotFound(new { error = "No readings found for this period" });

            var consumer = await _consumerRepo.GetByIdAsync(meter.ConsumerId);

            var totalConsumption = readings.Sum(r => r.ConsumptionKwh);
            var baseAmount = readings.Sum(r => r.ConsumptionKwh * r.BaseRate);
            var totalSurgeCharges = readings.Sum(r =>
                r.ConsumptionKwh * r.BaseRate * (r.SurgeChargePercent / 100));
            var totalDiscounts = readings.Sum(r =>
                r.ConsumptionKwh * r.BaseRate * (r.DiscountPercent / 100));
            var netAmount = baseAmount + totalSurgeCharges - totalDiscounts;
            var taxAmount = netAmount * 0.085m;
            var totalAmount = netAmount + taxAmount;

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            string billStatus = currentDate > endDate.AddDays(30) ? "Overdue" : "Pending";

            var bill = new MonthlyBillDetailDTO
            {
                BillId = $"{meterId}-{year}-{month}",
                MeterId = meterId,
                ConsumerId = meter.ConsumerId,
                ConsumerName = consumer?.Name ?? "N/A",
                BillingMonth = month,
                BillingYear = year,
                TotalConsumptionKwh = totalConsumption,
                BaseAmount = baseAmount,
                TotalSurgeCharges = totalSurgeCharges,
                TotalDiscounts = totalDiscounts,
                NetAmount = netAmount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                BillStatus = billStatus,
                ReadingsCount = readings.Count(),
                StartDate = startDate,
                EndDate = endDate
            };

            return Ok(bill);
        }

        [HttpGet("Summary")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> GetSummary()
        {
            var allReadings = await _readingRepo.GetAllAsync();

            var groupedBills = allReadings
                .GroupBy(r => new { r.MeterId, Year = r.ReadingDate.Year, Month = r.ReadingDate.Month })
                .Select(g => new
                {
                    g.Key.MeterId,
                    g.Key.Year,
                    g.Key.Month,
                    TotalAmount = g.Sum(r => r.ConsumptionKwh * r.BaseRate * (1 + (r.SurgeChargePercent - r.DiscountPercent) / 100)) * 1.085m
                })
                .ToList();

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var summary = new
            {
                totalBills = groupedBills.Count,
                totalAmount = groupedBills.Sum(b => b.TotalAmount),
                currentMonthBills = groupedBills.Count(b => b.Year == currentDate.Year && b.Month == currentDate.Month),
                currentMonthAmount = groupedBills
                    .Where(b => b.Year == currentDate.Year && b.Month == currentDate.Month)
                    .Sum(b => b.TotalAmount)
            };

            return Ok(summary);
        }

        private string GetMonthName(int month)
        {
            return new DateTime(2000, month, 1).ToString("MMMM");
        }
    }

    public class MonthlyBillDetailDTO
    {
        public string BillId { get; set; } = null!;
        public int MeterId { get; set; }
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
        public int ReadingsCount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}