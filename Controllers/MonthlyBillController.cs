using MDMS_Backend.Models;
using MDMS_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MonthlyBillController : ControllerBase
    {
        private readonly IMonthlyBillRepository _billRepo;

        public MonthlyBillController(IMonthlyBillRepository billRepo)
        {
            _billRepo = billRepo;
        }

        [HttpGet]
        [Route("AllBills")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<List<MonthlyBill>>> GetAllBills()
        {
            var bills = await _billRepo.GetAllAsync();
            return Ok(bills);
        }

        [HttpGet]
        [Route("ByMeterId/{meterId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<MonthlyBill>>> GetBillsByMeterId(int meterId)
        {
            var bills = await _billRepo.GetByMeterIdAsync(meterId);
            return Ok(bills);
        }

        [HttpGet]
        [Route("ByConsumerId/{consumerId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<MonthlyBill>>> GetBillsByConsumerId(int consumerId)
        {
            var bills = await _billRepo.GetByConsumerIdAsync(consumerId);
            return Ok(bills);
        }

        [HttpGet]
        [Route("ByMonth/{month:int}/{year:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<List<MonthlyBill>>> GetBillsByMonth(int month, int year)
        {
            var bills = await _billRepo.GetByMonthAsync(month, year);
            return Ok(bills);
        }

        [HttpGet]
        [Route("Filter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<List<MonthlyBill>>> GetFilteredBills(
            [FromQuery] int? meterId,
            [FromQuery] int? consumerId,
            [FromQuery] int? month,
            [FromQuery] int? year,
            [FromQuery] string? status)
        {
            var bills = await _billRepo.GetFilteredBillsAsync(meterId, consumerId, month, year, status);
            return Ok(bills);
        }

        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<int>> CreateBill([FromBody] MonthlyBillDTO model)
        {
            if (model == null)
            {
                return BadRequest("Bill data is required");
            }

            MonthlyBill billNew = new MonthlyBill
            {
                MeterId = model.MeterId,
                ConsumerId = model.ConsumerId,
                BillingMonth = model.BillingMonth,
                BillingYear = model.BillingYear,
                BillStartDate = model.BillStartDate,
                BillEndDate = model.BillEndDate,
                TotalConsumptionKwh = model.TotalConsumptionKwh,
                BaseAmount = model.BaseAmount,
                TotalSurgeCharges = model.TotalSurgeCharges,
                TotalDiscounts = model.TotalDiscounts,
                NetAmount = model.NetAmount,
                TaxAmount = model.TaxAmount,
                TotalAmount = model.TotalAmount,
                BillStatus = model.BillStatus,
                PaidDate = model.PaidDate,
                GeneratedAt = DateTime.Now,
                GeneratedBy = model.GeneratedBy
            };

            await _billRepo.CreateAsync(billNew);
            return Ok(billNew.BillId);
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateBill([FromBody] MonthlyBillDTO model)
        {
            if (model == null || model.BillId == 0)
            {
                return BadRequest("Invalid bill data");
            }

            var existingBill = await _billRepo.GetByIdAsync(model.BillId);
            if (existingBill == null)
            {
                return NotFound("Bill not found");
            }

            existingBill.MeterId = model.MeterId;
            existingBill.ConsumerId = model.ConsumerId;
            existingBill.BillingMonth = model.BillingMonth;
            existingBill.BillingYear = model.BillingYear;
            existingBill.BillStartDate = model.BillStartDate;
            existingBill.BillEndDate = model.BillEndDate;
            existingBill.TotalConsumptionKwh = model.TotalConsumptionKwh;
            existingBill.BaseAmount = model.BaseAmount;
            existingBill.TotalSurgeCharges = model.TotalSurgeCharges;
            existingBill.TotalDiscounts = model.TotalDiscounts;
            existingBill.NetAmount = model.NetAmount;
            existingBill.TaxAmount = model.TaxAmount;
            existingBill.TotalAmount = model.TotalAmount;
            existingBill.BillStatus = model.BillStatus;
            existingBill.PaidDate = model.PaidDate;
            existingBill.GeneratedBy = model.GeneratedBy;

            await _billRepo.UpdateAsync(existingBill);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteBill(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid bill ID");
            }

            var existingBill = await _billRepo.GetByIdAsync(id);
            if (existingBill == null)
            {
                return NotFound("Bill not found");
            }

            await _billRepo.DeleteAsync(id);
            return Ok();
        }
    }

    public class MonthlyBillDTO
    {
        public int BillId { get; set; }
        public int MeterId { get; set; }
        public int ConsumerId { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public DateOnly BillStartDate { get; set; }
        public DateOnly BillEndDate { get; set; }
        public decimal TotalConsumptionKwh { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal TotalSurgeCharges { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillStatus { get; set; } = null!;
        public DateTime? PaidDate { get; set; }
        public string GeneratedBy { get; set; } = null!;
    }
}