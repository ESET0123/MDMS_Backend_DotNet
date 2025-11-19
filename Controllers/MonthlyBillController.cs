//using MDMS_Backend.Models;
//using MDMS_Backend.Repositories;
//using MDMS_Backend.Repository;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
//using System.Threading.Tasks;

//namespace MDMS_Backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MonthlyBillController : ControllerBase
//    {
//        private readonly IMonthlyBillRepository _monthlyBillRepo;
//        private readonly IDailyMeterReadingRepository _dailyReadingRepo;

//        public MonthlyBillController(
//            IMonthlyBillRepository monthlyBillRepo,
//            IDailyMeterReadingRepository dailyReadingRepo)
//        {
//            _monthlyBillRepo = monthlyBillRepo;
//            _dailyReadingRepo = dailyReadingRepo;
//        }

//        [HttpGet("AllMonthlyBills")]
//        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
//        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetAllMonthlyBills()
//        {
//            var bills = await _monthlyBillRepo.GetAllAsync();

//            var dtos = bills.Select(b => new MonthlyBillDetailDTO
//            {
//                BillId = b.BillId,
//                ConsumerId = b.ConsumerId,
//                ConsumerName = b.Consumer?.Name ?? "N/A",
//                MeterId = b.MeterId,
//                TariffName = b.Meter?.Tariff?.Name ?? "N/A",
//                BillingDate = b.BillingDate,
//                TotalConsumptionKwh = b.TotalConsumptionKwh,
//                TotalAmount = b.TotalAmount,
//                BillStatus = b.BillStatus,
//                BillingMonth = b.BillingMonth,
//                BillingYear = b.BillingYear
//            });

//            return Ok(dtos);
//        }

//        [HttpGet("{id:int}")]
//        [ProducesResponseType(200, Type = typeof(MonthlyBillDetailDTO))]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult<MonthlyBillDetailDTO>> GetMonthlyBillById(int id)
//        {
//            var bill = await _monthlyBillRepo.GetByIdAsync(id);

//            if (bill == null)
//            {
//                return NotFound(new { error = "Bill not found" });
//            }

//            var dto = new MonthlyBillDetailDTO
//            {
//                BillId = bill.BillId,
//                ConsumerId = bill.ConsumerId,
//                ConsumerName = bill.Consumer?.Name ?? "N/A",
//                MeterId = bill.MeterId,
//                TariffName = bill.Meter?.Tariff?.Name ?? "N/A",
//                BillingDate = bill.BillingDate,
//                TotalConsumptionKwh = bill.TotalConsumptionKwh,
//                TotalAmount = bill.TotalAmount,
//                BillStatus = bill.BillStatus,
//                BillingMonth = bill.BillingMonth,
//                BillingYear = bill.BillingYear
//            };

//            return Ok(dto);
//        }

//        [HttpPost("Create")]
//        [ProducesResponseType(201)]
//        [ProducesResponseType(400)]
//        public async Task<ActionResult> CreateMonthlyBill([FromBody] MonthlyBillDTO model)
//        {
//            if (model == null || !ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var billNew = new MonthlyBill
//            {
//                ConsumerId = model.ConsumerId,
//                MeterId = model.MeterId,
//                BillingDate = model.BillingDate,
//                BillingMonth = model.BillingMonth,
//                BillingYear = model.BillingYear,
//                TotalConsumptionKwh = model.TotalConsumptionKwh,
//                TotalAmount = model.TotalAmount,
//                BillStatus = model.BillStatus
//            };

//            await _monthlyBillRepo.AddAsync(billNew);
//            return CreatedAtAction(nameof(GetMonthlyBillById), new { id = billNew.BillId }, billNew);
//        }

//        [HttpPut("Update")]
//        [ProducesResponseType(204)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult> UpdateMonthlyBill([FromBody] MonthlyBillDTO model)
//        {
//            if (model == null || model.BillId == null || model.BillId <= 0 || !ModelState.IsValid)
//            {
//                return BadRequest(new { error = "Invalid bill data" });
//            }

//            var existingBill = await _monthlyBillRepo.GetByIdAsync(model.BillId.Value);
//            if (existingBill == null)
//            {
//                return NotFound(new { error = "Bill not found" });
//            }

//            existingBill.ConsumerId = model.ConsumerId;
//            existingBill.MeterId = model.MeterId;
//            existingBill.BillingDate = model.BillingDate;
//            existingBill.BillingMonth = model.BillingMonth;
//            existingBill.BillingYear = model.BillingYear;
//            existingBill.TotalConsumptionKwh = model.TotalConsumptionKwh;
//            existingBill.TotalAmount = model.TotalAmount;
//            existingBill.BillStatus = model.BillStatus;

//            await _monthlyBillRepo.UpdateAsync(existingBill);
//            return NoContent();
//        }

//        [HttpDelete("{id:int}")]
//        [ProducesResponseType(204)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult> DeleteMonthlyBill(int id)
//        {
//            if (id <= 0)
//            {
//                return BadRequest(new { error = "Invalid bill ID" });
//            }

//            var existingBill = await _monthlyBillRepo.GetByIdAsync(id);
//            if (existingBill == null)
//            {
//                return NotFound(new { error = "Bill not found" });
//            }

//            await _monthlyBillRepo.DeleteAsync(id);
//            return NoContent();
//        }

//        [HttpPost("GenerateBills")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        public async Task<ActionResult<BillGenerationResult>> GenerateMonthlyBills([FromBody] BillGenerationRequest request)
//        {
//            if (request == null || !ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (request.Month < 1 || request.Month > 12)
//            {
//                return BadRequest(new { error = "Month must be between 1 and 12" });
//            }

//            if (request.Year < 2000 || request.Year > 2100)
//            {
//                return BadRequest(new { error = "Year must be between 2000 and 2100" });
//            }

//            try
//            {
//                var billsGenerated = await _monthlyBillRepo.GenerateMonthlyBillsAsync(request.Month, request.Year);

//                var result = new BillGenerationResult
//                {
//                    Month = request.Month,
//                    Year = request.Year,
//                    BillsGenerated = billsGenerated,
//                    Message = $"{billsGenerated} bills generated successfully for {request.Month}/{request.Year}"
//                };

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Error generating bills: {ex.Message}" });
//            }
//        }

//        [HttpPost("GenerateBillsFromReadings")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        public async Task<ActionResult<BillGenerationResult>> GenerateMonthlyBillsFromReadings([FromBody] BillGenerationRequest request)
//        {
//            if (request == null || !ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (request.Month < 1 || request.Month > 12)
//            {
//                return BadRequest(new { error = "Month must be between 1 and 12" });
//            }

//            if (request.Year < 2000 || request.Year > 2100)
//            {
//                return BadRequest(new { error = "Year must be between 2000 and 2100" });
//            }

//            try
//            {
//                var billsGenerated = await _monthlyBillRepo.GenerateMonthlyBillsFromReadingsAsync(request.Month, request.Year);

//                var result = new BillGenerationResult
//                {
//                    Month = request.Month,
//                    Year = request.Year,
//                    BillsGenerated = billsGenerated,
//                    Message = $"Bills generated from daily readings for {request.Month}/{request.Year}"
//                };

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Error generating bills from readings: {ex.Message}" });
//            }
//        }

//        // NEW: Generate bill for specific meter
//        [HttpPost("GenerateBillForMeter")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        public async Task<ActionResult<BillGenerationResult>> GenerateBillForMeter([FromBody] MeterBillGenerationRequest request)
//        {
//            if (request == null || !ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (request.MeterId <= 0)
//            {
//                return BadRequest(new { error = "Invalid meter ID" });
//            }

//            if (request.Month < 1 || request.Month > 12)
//            {
//                return BadRequest(new { error = "Month must be between 1 and 12" });
//            }

//            if (request.Year < 2000 || request.Year > 2100)
//            {
//                return BadRequest(new { error = "Year must be between 2000 and 2100" });
//            }

//            try
//            {
//                var billsGenerated = await _monthlyBillRepo.GenerateBillForMeterAsync(request.MeterId, request.Month, request.Year);

//                var result = new BillGenerationResult
//                {
//                    Month = request.Month,
//                    Year = request.Year,
//                    BillsGenerated = billsGenerated,
//                    Message = billsGenerated > 0
//                        ? $"Bill generated successfully for Meter #{request.MeterId} for {request.Month}/{request.Year}"
//                        : $"No bill generated. Please check if daily readings exist for Meter #{request.MeterId} in {request.Month}/{request.Year}"
//                };

//                return Ok(result);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(new { error = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Error generating bill: {ex.Message}" });
//            }
//        }

//        // NEW: Update bill status with sequential validation
//        [HttpPut("UpdateBillStatus/{billId}")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult> UpdateBillStatus(int billId, [FromBody] BillStatusUpdateRequest request)
//        {
//            if (request == null || !ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var bill = await _monthlyBillRepo.GetByIdAsync(billId);
//            if (bill == null)
//            {
//                return NotFound(new { error = "Bill not found" });
//            }

//            // Validate sequential payment if trying to mark as Paid
//            if (request.BillStatus == "Paid" && bill.BillStatus != "Paid")
//            {
//                var validationResult = await _monthlyBillRepo.ValidateSequentialPaymentAsync(bill.MeterId, bill.BillingMonth, bill.BillingYear);

//                if (!validationResult.IsValid)
//                {
//                    return BadRequest(new { error = validationResult.ErrorMessage });
//                }
//            }

//            bill.BillStatus = request.BillStatus;
//            await _monthlyBillRepo.UpdateAsync(bill);

//            return Ok(new
//            {
//                message = "Bill status updated successfully",
//                billId = billId,
//                newStatus = request.BillStatus
//            });
//        }

//        [HttpGet("ByMeterAndMonth/{meterId}/{month}/{year}")]
//        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
//        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetBillsByMeterAndMonth(int meterId, int month, int year)
//        {
//            var bills = await _monthlyBillRepo.GetByMeterAndMonthAsync(meterId, month, year);

//            var dtos = bills.Select(b => new MonthlyBillDetailDTO
//            {
//                BillId = b.BillId,
//                ConsumerId = b.ConsumerId,
//                ConsumerName = b.Consumer?.Name ?? "N/A",
//                MeterId = b.MeterId,
//                TariffName = b.Meter?.Tariff?.Name ?? "N/A",
//                BillingDate = b.BillingDate,
//                TotalConsumptionKwh = b.TotalConsumptionKwh,
//                TotalAmount = b.TotalAmount,
//                BillStatus = b.BillStatus,
//                BillingMonth = b.BillingMonth,
//                BillingYear = b.BillingYear
//            });

//            return Ok(dtos);
//        }

//        [HttpGet("ByConsumer/{consumerId}")]
//        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
//        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetBillsByConsumer(int consumerId)
//        {
//            var bills = await _monthlyBillRepo.GetByConsumerAsync(consumerId);

//            var dtos = bills.Select(b => new MonthlyBillDetailDTO
//            {
//                BillId = b.BillId,
//                ConsumerId = b.ConsumerId,
//                ConsumerName = b.Consumer?.Name ?? "N/A",
//                MeterId = b.MeterId,
//                TariffName = b.Meter?.Tariff?.Name ?? "N/A",
//                BillingDate = b.BillingDate,
//                TotalConsumptionKwh = b.TotalConsumptionKwh,
//                TotalAmount = b.TotalAmount,
//                BillStatus = b.BillStatus,
//                BillingMonth = b.BillingMonth,
//                BillingYear = b.BillingYear
//            });

//            return Ok(dtos);
//        }

//        [HttpGet("CheckBillsExist/{month}/{year}")]
//        [ProducesResponseType(200, Type = typeof(bool))]
//        public async Task<ActionResult<bool>> CheckBillsExist(int month, int year)
//        {
//            var exists = await _monthlyBillRepo.CheckBillsExistForMonthAsync(month, year);
//            return Ok(exists);
//        }

//        [HttpGet("DailyReadings/{meterId}/{month}/{year}")]
//        [ProducesResponseType(200, Type = typeof(IEnumerable<DailyReadingDTO>))]
//        public async Task<ActionResult<IEnumerable<DailyReadingDTO>>> GetDailyReadingsForMonth(int meterId, int month, int year)
//        {
//            var startDate = new DateOnly(year, month, 1);
//            var endDate = startDate.AddMonths(1).AddDays(-1);

//            var readings = await _dailyReadingRepo.GetByMeterAndDateRangeAsync(meterId, startDate, endDate);

//            var dtos = readings.Select(r => new DailyReadingDTO
//            {
//                ReadingId = r.ReadingId,
//                MeterId = r.MeterId,
//                ReadingDate = r.ReadingDate,
//                TodRuleId = r.TodRuleId,
//                TodRuleName = r.TodRule?.RuleName ?? "N/A",
//                StartTime = r.StartTime,
//                EndTime = r.EndTime,
//                PreviousReading = r.PreviousReading,
//                CurrentReading = r.CurrentReading,
//                ConsumptionKwh = r.ConsumptionKwh,
//                BaseRate = r.BaseRate,
//                SurgeChargePercent = r.SurgeChargePercent,
//                DiscountPercent = r.DiscountPercent,
//                EffectiveRate = r.EffectiveRate,
//                Amount = r.Amount
//            });

//            return Ok(dtos);
//        }
//    }

//    // DTOs
//    public class BillGenerationRequest
//    {
//        [Required]
//        [Range(1, 12)]
//        public int Month { get; set; }

//        [Required]
//        [Range(2000, 2100)]
//        public int Year { get; set; }
//    }

//    public class MeterBillGenerationRequest
//    {
//        [Required]
//        public int MeterId { get; set; }

//        [Required]
//        [Range(1, 12)]
//        public int Month { get; set; }

//        [Required]
//        [Range(2000, 2100)]
//        public int Year { get; set; }
//    }

//    public class BillStatusUpdateRequest
//    {
//        [Required]
//        [StringLength(50)]
//        public string BillStatus { get; set; } = null!;
//    }

//    public class BillGenerationResult
//    {
//        public int Month { get; set; }
//        public int Year { get; set; }
//        public int BillsGenerated { get; set; }
//        public string Message { get; set; } = null!;
//    }

//    public class DailyReadingDTO
//    {
//        public int ReadingId { get; set; }
//        public int MeterId { get; set; }
//        public DateOnly ReadingDate { get; set; }
//        public int TodRuleId { get; set; }
//        public string TodRuleName { get; set; } = null!;
//        public TimeOnly StartTime { get; set; }
//        public TimeOnly EndTime { get; set; }
//        public decimal PreviousReading { get; set; }
//        public decimal CurrentReading { get; set; }
//        public decimal ConsumptionKwh { get; set; }
//        public decimal BaseRate { get; set; }
//        public decimal SurgeChargePercent { get; set; }
//        public decimal DiscountPercent { get; set; }
//        public decimal EffectiveRate { get; set; }
//        public decimal Amount { get; set; }
//    }

//    public class MonthlyBillDTO
//    {
//        public int? BillId { get; set; }

//        [Required]
//        public int ConsumerId { get; set; }

//        [Required]
//        public int MeterId { get; set; }

//        [Required]
//        public DateOnly BillingDate { get; set; }

//        [Required]
//        [Range(1, 12)]
//        public int BillingMonth { get; set; }

//        [Required]
//        [Range(2000, 2100)]
//        public int BillingYear { get; set; }

//        [Required]
//        [Range(0, double.MaxValue)]
//        public decimal TotalConsumptionKwh { get; set; }

//        [Required]
//        [Range(0, double.MaxValue)]
//        public decimal TotalAmount { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string BillStatus { get; set; } = null!;
//    }

//    public class MonthlyBillDetailDTO
//    {
//        public int BillId { get; set; }
//        public int ConsumerId { get; set; }
//        public string ConsumerName { get; set; } = null!;
//        public int MeterId { get; set; }
//        public string TariffName { get; set; } = null!;
//        public DateOnly BillingDate { get; set; }
//        public decimal TotalConsumptionKwh { get; set; }
//        public decimal TotalAmount { get; set; }
//        public string BillStatus { get; set; } = null!;
//        public int BillingMonth { get; set; }
//        public int BillingYear { get; set; }
//    }
//}

using MDMS_Backend.Models;
using MDMS_Backend.Repositories;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyBillController : ControllerBase
    {
        private readonly IMonthlyBillRepository _monthlyBillRepo;
        private readonly IDailyMeterReadingRepository _dailyReadingRepo;

        public MonthlyBillController(
            IMonthlyBillRepository monthlyBillRepo,
            IDailyMeterReadingRepository dailyReadingRepo)
        {
            _monthlyBillRepo = monthlyBillRepo;
            _dailyReadingRepo = dailyReadingRepo;
        }

        [HttpGet("AllMonthlyBills")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetAllMonthlyBills()
        {
            var bills = await _monthlyBillRepo.GetAllAsync();

            var dtos = bills.Select(b => new MonthlyBillDetailDTO
            {
                BillId = b.BillId,
                ConsumerId = b.ConsumerId,
                ConsumerName = b.Consumer?.Name ?? "N/A",
                MeterId = b.MeterId,
                TariffName = b.Meter?.Tariff?.Name ?? "N/A",
                BillingDate = b.BillingDate,
                TotalConsumptionKwh = b.TotalConsumptionKwh,
                TotalAmount = b.TotalAmount,
                BillStatus = b.BillStatus,
                BillingMonth = b.BillingMonth,
                BillingYear = b.BillingYear
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(MonthlyBillDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MonthlyBillDetailDTO>> GetMonthlyBillById(int id)
        {
            var bill = await _monthlyBillRepo.GetByIdAsync(id);

            if (bill == null)
            {
                return NotFound(new { error = "Bill not found" });
            }

            var dto = new MonthlyBillDetailDTO
            {
                BillId = bill.BillId,
                ConsumerId = bill.ConsumerId,
                ConsumerName = bill.Consumer?.Name ?? "N/A",
                MeterId = bill.MeterId,
                TariffName = bill.Meter?.Tariff?.Name ?? "N/A",
                BillingDate = bill.BillingDate,
                TotalConsumptionKwh = bill.TotalConsumptionKwh,
                TotalAmount = bill.TotalAmount,
                BillStatus = bill.BillStatus,
                BillingMonth = bill.BillingMonth,
                BillingYear = bill.BillingYear
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateMonthlyBill([FromBody] MonthlyBillDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate sequential payment for new bills marked as Paid
            if (model.BillStatus == "Paid")
            {
                var validationResult = await _monthlyBillRepo.ValidateSequentialPaymentAsync(model.MeterId, model.BillingMonth, model.BillingYear);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
            }

            var billNew = new MonthlyBill
            {
                ConsumerId = model.ConsumerId,
                MeterId = model.MeterId,
                BillingDate = model.BillingDate,
                BillingMonth = model.BillingMonth,
                BillingYear = model.BillingYear,
                TotalConsumptionKwh = model.TotalConsumptionKwh,
                TotalAmount = model.TotalAmount,
                BillStatus = model.BillStatus
            };

            await _monthlyBillRepo.AddAsync(billNew);
            return CreatedAtAction(nameof(GetMonthlyBillById), new { id = billNew.BillId }, billNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateMonthlyBill([FromBody] MonthlyBillDTO model)
        {
            if (model == null || model.BillId == null || model.BillId <= 0 || !ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid bill data" });
            }

            var existingBill = await _monthlyBillRepo.GetByIdAsync(model.BillId.Value);
            if (existingBill == null)
            {
                return NotFound(new { error = "Bill not found" });
            }

            // Validate sequential payment if changing status to Paid
            if (model.BillStatus == "Paid" && existingBill.BillStatus != "Paid")
            {
                var validationResult = await _monthlyBillRepo.ValidateSequentialPaymentAsync(model.MeterId, model.BillingMonth, model.BillingYear);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
            }

            existingBill.ConsumerId = model.ConsumerId;
            existingBill.MeterId = model.MeterId;
            existingBill.BillingDate = model.BillingDate;
            existingBill.BillingMonth = model.BillingMonth;
            existingBill.BillingYear = model.BillingYear;
            existingBill.TotalConsumptionKwh = model.TotalConsumptionKwh;
            existingBill.TotalAmount = model.TotalAmount;
            existingBill.BillStatus = model.BillStatus;

            await _monthlyBillRepo.UpdateAsync(existingBill);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteMonthlyBill(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid bill ID" });
            }

            var existingBill = await _monthlyBillRepo.GetByIdAsync(id);
            if (existingBill == null)
            {
                return NotFound(new { error = "Bill not found" });
            }

            await _monthlyBillRepo.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("GenerateBills")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BillGenerationResult>> GenerateMonthlyBills([FromBody] BillGenerationRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Month < 1 || request.Month > 12)
            {
                return BadRequest(new { error = "Month must be between 1 and 12" });
            }

            if (request.Year < 2000 || request.Year > 2100)
            {
                return BadRequest(new { error = "Year must be between 2000 and 2100" });
            }

            try
            {
                var billsGenerated = await _monthlyBillRepo.GenerateMonthlyBillsAsync(request.Month, request.Year);

                var result = new BillGenerationResult
                {
                    Month = request.Month,
                    Year = request.Year,
                    BillsGenerated = billsGenerated,
                    Message = $"{billsGenerated} bills generated successfully for {request.Month}/{request.Year}"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error generating bills: {ex.Message}" });
            }
        }

        [HttpPost("GenerateBillsFromReadings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BillGenerationResult>> GenerateMonthlyBillsFromReadings([FromBody] BillGenerationRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Month < 1 || request.Month > 12)
            {
                return BadRequest(new { error = "Month must be between 1 and 12" });
            }

            if (request.Year < 2000 || request.Year > 2100)
            {
                return BadRequest(new { error = "Year must be between 2000 and 2100" });
            }

            try
            {
                var billsGenerated = await _monthlyBillRepo.GenerateMonthlyBillsFromReadingsAsync(request.Month, request.Year);

                var result = new BillGenerationResult
                {
                    Month = request.Month,
                    Year = request.Year,
                    BillsGenerated = billsGenerated,
                    Message = $"Bills generated from daily readings for {request.Month}/{request.Year}"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error generating bills from readings: {ex.Message}" });
            }
        }

        [HttpPost("GenerateBillForMeter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BillGenerationResult>> GenerateBillForMeter([FromBody] MeterBillGenerationRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.MeterId <= 0)
            {
                return BadRequest(new { error = "Invalid meter ID" });
            }

            if (request.Month < 1 || request.Month > 12)
            {
                return BadRequest(new { error = "Month must be between 1 and 12" });
            }

            if (request.Year < 2000 || request.Year > 2100)
            {
                return BadRequest(new { error = "Year must be between 2000 and 2100" });
            }

            try
            {
                var billsGenerated = await _monthlyBillRepo.GenerateBillForMeterAsync(request.MeterId, request.Month, request.Year);

                var result = new BillGenerationResult
                {
                    Month = request.Month,
                    Year = request.Year,
                    BillsGenerated = billsGenerated,
                    Message = billsGenerated > 0
                        ? $"Bill generated successfully for Meter #{request.MeterId} for {request.Month}/{request.Year}"
                        : $"No bill generated. Please check if daily readings exist for Meter #{request.MeterId} in {request.Month}/{request.Year}"
                };

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error generating bill: {ex.Message}" });
            }
        }

        [HttpPut("UpdateBillStatus/{billId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateBillStatus(int billId, [FromBody] BillStatusUpdateRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bill = await _monthlyBillRepo.GetByIdAsync(billId);
            if (bill == null)
            {
                return NotFound(new { error = "Bill not found" });
            }

            // Validate sequential payment if trying to mark as Paid
            if (request.BillStatus == "Paid" && bill.BillStatus != "Paid")
            {
                var validationResult = await _monthlyBillRepo.ValidateSequentialPaymentAsync(bill.MeterId, bill.BillingMonth, bill.BillingYear);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
            }

            bill.BillStatus = request.BillStatus;
            await _monthlyBillRepo.UpdateAsync(bill);

            return Ok(new
            {
                message = "Bill status updated successfully",
                billId = billId,
                newStatus = request.BillStatus
            });
        }

        [HttpGet("ByMeterAndMonth/{meterId}/{month}/{year}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetBillsByMeterAndMonth(int meterId, int month, int year)
        {
            var bills = await _monthlyBillRepo.GetByMeterAndMonthAsync(meterId, month, year);

            var dtos = bills.Select(b => new MonthlyBillDetailDTO
            {
                BillId = b.BillId,
                ConsumerId = b.ConsumerId,
                ConsumerName = b.Consumer?.Name ?? "N/A",
                MeterId = b.MeterId,
                TariffName = b.Meter?.Tariff?.Name ?? "N/A",
                BillingDate = b.BillingDate,
                TotalConsumptionKwh = b.TotalConsumptionKwh,
                TotalAmount = b.TotalAmount,
                BillStatus = b.BillStatus,
                BillingMonth = b.BillingMonth,
                BillingYear = b.BillingYear
            });

            return Ok(dtos);
        }

        [HttpGet("ByConsumer/{consumerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MonthlyBillDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MonthlyBillDetailDTO>>> GetBillsByConsumer(int consumerId)
        {
            var bills = await _monthlyBillRepo.GetByConsumerAsync(consumerId);

            var dtos = bills.Select(b => new MonthlyBillDetailDTO
            {
                BillId = b.BillId,
                ConsumerId = b.ConsumerId,
                ConsumerName = b.Consumer?.Name ?? "N/A",
                MeterId = b.MeterId,
                TariffName = b.Meter?.Tariff?.Name ?? "N/A",
                BillingDate = b.BillingDate,
                TotalConsumptionKwh = b.TotalConsumptionKwh,
                TotalAmount = b.TotalAmount,
                BillStatus = b.BillStatus,
                BillingMonth = b.BillingMonth,
                BillingYear = b.BillingYear
            });

            return Ok(dtos);
        }

        [HttpGet("CheckBillsExist/{month}/{year}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<ActionResult<bool>> CheckBillsExist(int month, int year)
        {
            var exists = await _monthlyBillRepo.CheckBillsExistForMonthAsync(month, year);
            return Ok(exists);
        }

        [HttpGet("DailyReadings/{meterId}/{month}/{year}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DailyReadingDTO>))]
        public async Task<ActionResult<IEnumerable<DailyReadingDTO>>> GetDailyReadingsForMonth(int meterId, int month, int year)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var readings = await _dailyReadingRepo.GetByMeterAndDateRangeAsync(meterId, startDate, endDate);

            var dtos = readings.Select(r => new DailyReadingDTO
            {
                ReadingId = r.ReadingId,
                MeterId = r.MeterId,
                ReadingDate = r.ReadingDate,
                TodRuleId = r.TodRuleId,
                TodRuleName = r.TodRule?.RuleName ?? "N/A",
                StartTime = r.StartTime,
                EndTime = r.EndTime,
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

        [HttpGet("ValidateSequentialPayment/{billId}")]
        [ProducesResponseType(200, Type = typeof(BillValidationResult))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BillValidationResult>> ValidateSequentialPayment(int billId)
        {
            try
            {
                var bill = await _monthlyBillRepo.GetByIdAsync(billId);
                if (bill == null)
                {
                    return NotFound(new { error = "Bill not found" });
                }

                var validationResult = await _monthlyBillRepo.ValidateSequentialPaymentAsync(bill.MeterId, bill.BillingMonth, bill.BillingYear);
                return Ok(validationResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error validating payment sequence: {ex.Message}" });
            }
        }
    }

    // DTOs
    public class BillGenerationRequest
    {
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }
    }

    public class MeterBillGenerationRequest
    {
        [Required]
        public int MeterId { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }
    }

    public class BillStatusUpdateRequest
    {
        [Required]
        [StringLength(50)]
        public string BillStatus { get; set; } = null!;
    }

    public class BillGenerationResult
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int BillsGenerated { get; set; }
        public string Message { get; set; } = null!;
    }

    public class DailyReadingDTO
    {
        public int ReadingId { get; set; }
        public int MeterId { get; set; }
        public DateOnly ReadingDate { get; set; }
        public int TodRuleId { get; set; }
        public string TodRuleName { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal ConsumptionKwh { get; set; }
        public decimal BaseRate { get; set; }
        public decimal SurgeChargePercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal EffectiveRate { get; set; }
        public decimal Amount { get; set; }
    }

    public class MonthlyBillDTO
    {
        public int? BillId { get; set; }

        [Required]
        public int ConsumerId { get; set; }

        [Required]
        public int MeterId { get; set; }

        [Required]
        public DateOnly BillingDate { get; set; }

        [Required]
        [Range(1, 12)]
        public int BillingMonth { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int BillingYear { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalConsumptionKwh { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string BillStatus { get; set; } = null!;
    }

    public class MonthlyBillDetailDTO
    {
        public int BillId { get; set; }
        public int ConsumerId { get; set; }
        public string ConsumerName { get; set; } = null!;
        public int MeterId { get; set; }
        public string TariffName { get; set; } = null!;
        public DateOnly BillingDate { get; set; }
        public decimal TotalConsumptionKwh { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillStatus { get; set; } = null!;
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
    }
}