using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] 
    public class TariffSlabController : ControllerBase
    {
        private readonly ITariffSlabRepository _tariffSlabRepo;

        public TariffSlabController(ITariffSlabRepository tariffSlabRepo)
        {
            _tariffSlabRepo = tariffSlabRepo;
        }

        [HttpGet("AllTariffSlabs")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TariffSlabDetailDTO>))]
        public async Task<ActionResult<IEnumerable<TariffSlabDetailDTO>>> GetAllTariffSlabs()
        {
            var slabs = await _tariffSlabRepo.GetAllAsync();

            var dtos = slabs.Select(s => new TariffSlabDetailDTO
            {
                SlabId = s.SlabId,
                TariffId = s.TariffId,
                TariffName = s.Tariff?.Name ?? "Unknown Tariff",
                FromKwh = s.FromKwh,
                ToKwh = s.ToKwh,
                RatePerKwh = s.RatePerKwh
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(TariffSlabDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TariffSlabDetailDTO>> GetTariffSlabById(int id)
        {
            var slab = await _tariffSlabRepo.GetByIdAsync(id);

            if (slab == null)
            {
                return NotFound();
            }

            var dto = new TariffSlabDetailDTO
            {
                SlabId = slab.SlabId,
                TariffId = slab.TariffId,
                TariffName = slab.Tariff?.Name ?? "Unknown Tariff",
                FromKwh = slab.FromKwh,
                ToKwh = slab.ToKwh,
                RatePerKwh = slab.RatePerKwh
            };

            return Ok(dto);
        }

        [HttpGet("ByTariff/{tariffId:int}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TariffSlabDetailDTO>))]
        public async Task<ActionResult<IEnumerable<TariffSlabDetailDTO>>> GetTariffSlabsByTariffId(int tariffId)
        {
            var slabs = await _tariffSlabRepo.GetByTariffIdAsync(tariffId);

            var dtos = slabs.Select(s => new TariffSlabDetailDTO
            {
                SlabId = s.SlabId,
                TariffId = s.TariffId,
                TariffName = s.Tariff?.Name ?? "Unknown Tariff",
                FromKwh = s.FromKwh,
                ToKwh = s.ToKwh,
                RatePerKwh = s.RatePerKwh
            });

            return Ok(dtos);
        }


        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateTariffSlab([FromBody] TariffSlabDTO model)
        {
            if (model.ToKwh <= model.FromKwh)
            {
                ModelState.AddModelError("ToKwh", "ToKwh must be greater than FromKwh.");
            }

            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tariffSlabNew = new TariffSlab
            {
                TariffId = model.TariffId,
                FromKwh = model.FromKwh,
                ToKwh = model.ToKwh,
                RatePerKwh = model.RatePerKwh
            };

            await _tariffSlabRepo.AddAsync(tariffSlabNew);
            return CreatedAtAction(nameof(GetTariffSlabById), new { id = tariffSlabNew.SlabId }, tariffSlabNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateTariffSlab([FromBody] TariffSlabDTO model)
        {
            if (model.ToKwh <= model.FromKwh)
            {
                ModelState.AddModelError("ToKwh", "ToKwh must be greater than FromKwh.");
            }

            if (model == null || model.SlabId == null || model.SlabId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingTariffSlab = await _tariffSlabRepo.GetByIdAsync(model.SlabId.Value);
            if (existingTariffSlab == null)
            {
                return NotFound();
            }

            var tariffSlabUpdate = new TariffSlab
            {
                SlabId = model.SlabId.Value,
                TariffId = model.TariffId,
                FromKwh = model.FromKwh,
                ToKwh = model.ToKwh,
                RatePerKwh = model.RatePerKwh
            };

            await _tariffSlabRepo.UpdateAsync(tariffSlabUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteTariffSlab(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingTariffSlab = await _tariffSlabRepo.GetByIdAsync(id);
            if (existingTariffSlab == null)
            {
                return NotFound();
            }

            await _tariffSlabRepo.DeleteAsync(id);
            return NoContent();
        }
    }
    public class TariffSlabDTO
    {
        public int? SlabId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TariffId must be a positive integer.")]
        public int TariffId { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "FromKwh must be a positive number.")]
        public decimal FromKwh { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "ToKwh must be a positive number.")]
        public decimal ToKwh { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "RatePerKwh must be a positive number.")]
        public decimal RatePerKwh { get; set; }
    }

    public class TariffSlabDetailDTO
    {
        public int SlabId { get; set; }
        public int TariffId { get; set; }
        public string TariffName { get; set; } = null!;
        public decimal FromKwh { get; set; }
        public decimal ToKwh { get; set; }
        public decimal RatePerKwh { get; set; }
    }
}
