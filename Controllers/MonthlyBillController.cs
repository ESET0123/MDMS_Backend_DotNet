using Microsoft.AspNetCore.Mvc;
using MDMS_Backend.Models;
using MDMS_Backend.Repositories;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyBillController : ControllerBase
    {
        private readonly IMonthlyBillRepository _monthlyBillRepo;

        public MonthlyBillController(IMonthlyBillRepository monthlyBillRepo)
        {
            _monthlyBillRepo = monthlyBillRepo;
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
                BillingDate = b.BillingDate,
                TotalConsumptionKwh = b.TotalConsumptionKwh,
                TotalAmount = b.TotalAmount,
                BillStatus = b.BillStatus
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
                return NotFound();
            }

            var dto = new MonthlyBillDetailDTO
            {
                BillId = bill.BillId,
                ConsumerId = bill.ConsumerId,
                ConsumerName = bill.Consumer?.Name ?? "N/A",
                MeterId = bill.MeterId,
                BillingDate = bill.BillingDate,
                TotalConsumptionKwh = bill.TotalConsumptionKwh,
                TotalAmount = bill.TotalAmount,
                BillStatus = bill.BillStatus
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

            var billNew = new MonthlyBill
            {
                ConsumerId = model.ConsumerId,
                MeterId = model.MeterId,
                BillingDate = model.BillingDate,
                TotalConsumptionKwh = model.TotalConsumptionKwh,
                TotalAmount = model.TotalAmount,
                BillStatus = model.BillStatus
            };

            await _monthlyBillRepo.AddAsync(billNew);
            return CreatedAtAction(nameof(GetMonthlyBillById), new { id = billNew.BillId }, billNew);
        }

        //[HttpPut("Update")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<ActionResult> UpdateMonthlyBill([FromBody] MonthlyBillDTO model)
        //{
        //    if (model == null || model.BillId == null || model.BillId <= 0 || !ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    var existingBill = await _monthlyBillRepo.GetByIdAsync(model.BillId.Value);
        //    if (existingBill == null)
        //    {
        //        return NotFound();
        //    }

        //    var billUpdate = new MonthlyBill
        //    {
        //        BillId = model.BillId.Value,
        //        ConsumerId = model.ConsumerId,
        //        MeterId = model.MeterId,
        //        BillingDate = model.BillingDate,
        //        TotalConsumptionKwh = model.TotalConsumptionKwh,
        //        TotalAmount = model.TotalAmount,
        //        BillStatus = model.BillStatus
        //    };

        //    await _monthlyBillRepo.UpdateAsync(billUpdate);
        //    return NoContent();
        //}
        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateMonthlyBill([FromBody] MonthlyBillDTO model)
        {
            // ... (Your validation code remains the same) ...
            if (model == null || model.BillId == null || model.BillId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingBill = await _monthlyBillRepo.GetByIdAsync(model.BillId.Value);
            if (existingBill == null)
            {
                return NotFound();
            }

            // --- FIX IS HERE: Update the properties of the existing, tracked entity ---
            existingBill.ConsumerId = model.ConsumerId;
            existingBill.MeterId = model.MeterId;
            existingBill.BillingDate = model.BillingDate;
            existingBill.TotalConsumptionKwh = model.TotalConsumptionKwh;
            existingBill.TotalAmount = model.TotalAmount;
            existingBill.BillStatus = model.BillStatus;

            // The UpdateAsync method no longer needs to manually set the state to Modified,
            // as EF Core tracks changes automatically.
            // We should slightly refactor the repository method to just call SaveChanges().
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
                return BadRequest();
            }

            var existingBill = await _monthlyBillRepo.GetByIdAsync(id);
            if (existingBill == null)
            {
                return NotFound();
            }

            await _monthlyBillRepo.DeleteAsync(id);
            return NoContent();
        }
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
        public DateOnly BillingDate { get; set; }
        public decimal TotalConsumptionKwh { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillStatus { get; set; } = null!;
    }
}