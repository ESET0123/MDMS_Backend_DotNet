using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] 
    public class TariffController : ControllerBase
    {
        private readonly ITariffRepository _tariffRepo;

        public TariffController(ITariffRepository tariffRepo)
        {
            _tariffRepo = tariffRepo;
        }

        [HttpGet("AllTariffs")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Tariff>))]
        public async Task<ActionResult<IEnumerable<Tariff>>> GetTariffs()
        {
            var tariffs = await _tariffRepo.GetAllAsync();
            return Ok(tariffs);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(Tariff))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Tariff>> GetTariffById(int id)
        {
            var tariff = await _tariffRepo.GetByIdAsync(id);

            if (tariff == null)
            {
                return NotFound();
            }

            return Ok(tariff);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateTariff([FromBody] TariffDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tariffNew = new Tariff
            {
                Name = model.Name,
                EffectiveFrom = DateOnly.FromDateTime(model.EffectiveFrom),
                EffectiveTo = model.EffectiveTo.HasValue ? DateOnly.FromDateTime(model.EffectiveTo.Value) : (DateOnly?)null,
                BaseRate = model.BaseRate,
                TaxRate = model.TaxRate
            };

            await _tariffRepo.AddAsync(tariffNew);
            return CreatedAtAction(nameof(GetTariffById), new { id = tariffNew.TariffId }, tariffNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateTariff([FromBody] TariffDTO model)
        {
            if (model == null || model.TariffId == null || model.TariffId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingTariff = await _tariffRepo.GetByIdAsync(model.TariffId.Value);
            if (existingTariff == null)
            {
                return NotFound();
            }

            var tariffUpdate = new Tariff
            {
                TariffId = model.TariffId.Value,
                Name = model.Name,
                EffectiveFrom = DateOnly.FromDateTime(model.EffectiveFrom),
                EffectiveTo = model.EffectiveTo.HasValue ? DateOnly.FromDateTime(model.EffectiveTo.Value) : (DateOnly?)null,
                BaseRate = model.BaseRate,
                TaxRate = model.TaxRate
            };

            await _tariffRepo.UpdateAsync(tariffUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteTariff(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingTariff = await _tariffRepo.GetByIdAsync(id);
            if (existingTariff == null)
            {
                return NotFound();
            }

            await _tariffRepo.DeleteAsync(id);
            return NoContent();
        }
    }
    public class TariffDTO
    {
        public int? TariffId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal BaseRate { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Tax Rate must be between 0 and 100.")]
        public decimal TaxRate { get; set; }
    }
}
