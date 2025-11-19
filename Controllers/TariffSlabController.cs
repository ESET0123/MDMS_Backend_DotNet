using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
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
                RatePerKwh = s.RatePerKwh,
                FromDate = s.FromDate,  
                ToDate = s.ToDate       
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
                RatePerKwh = slab.RatePerKwh,
                FromDate = slab.FromDate,  
                ToDate = slab.ToDate       
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
                RatePerKwh = s.RatePerKwh,
                FromDate = s.FromDate,  
                ToDate = s.ToDate       
            });

            return Ok(dtos);
        }


        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateTariffSlab([FromBody] TariffSlabDTO model)
        {
            // Basic validation
            if (model.ToKwh <= model.FromKwh)
            {
                ModelState.AddModelError("ToKwh", "ToKwh must be greater than FromKwh.");
            }

            if (model.ToDate <= model.FromDate)
            {
                ModelState.AddModelError("ToDate", "ToDate must be greater than FromDate.");
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
                RatePerKwh = model.RatePerKwh,
                FromDate = DateOnly.FromDateTime(model.FromDate),
                ToDate = DateOnly.FromDateTime(model.ToDate)
            };

            // Check for overlapping slabs
            bool hasOverlap = await _tariffSlabRepo.HasOverlappingSlabAsync(tariffSlabNew);
            if (hasOverlap)
            {
                // Get the overlapping slabs for detailed error message
                var overlappingSlabs = await _tariffSlabRepo.GetPotentialOverlapsAsync(
                    model.TariffId,
                    DateOnly.FromDateTime(model.FromDate),
                    DateOnly.FromDateTime(model.ToDate),
                    model.FromKwh,
                    model.ToKwh
                );

                var errorMessage = overlappingSlabs.Any()
                    ? $"This tariff slab overlaps with existing slab(s): {string.Join(", ", overlappingSlabs.Select(s => $"Slab {s.SlabId} ({s.FromDate} to {s.ToDate}, {s.FromKwh}-{s.ToKwh}kWh)"))}"
                    : "This tariff slab overlaps with an existing slab in terms of date range and consumption range for the same tariff type.";

                ModelState.AddModelError("", errorMessage);
                return BadRequest(ModelState);
            }

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

            if (model.ToDate <= model.FromDate)
            {
                ModelState.AddModelError("ToDate", "ToDate must be greater than FromDate.");
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
                RatePerKwh = model.RatePerKwh,
                FromDate = DateOnly.FromDateTime(model.FromDate),
                ToDate = DateOnly.FromDateTime(model.ToDate)
            };

            // Check for overlapping slabs (excluding current slab being updated)
            bool hasOverlap = await _tariffSlabRepo.HasOverlappingSlabAsync(tariffSlabUpdate, model.SlabId.Value);
            if (hasOverlap)
            {
                // Get the overlapping slabs for detailed error message
                var overlappingSlabs = await _tariffSlabRepo.GetPotentialOverlapsAsync(
                    model.TariffId,
                    DateOnly.FromDateTime(model.FromDate),
                    DateOnly.FromDateTime(model.ToDate),
                    model.FromKwh,
                    model.ToKwh,
                    model.SlabId.Value
                );

                var errorMessage = overlappingSlabs.Any()
                    ? $"This tariff slab overlaps with existing slab(s): {string.Join(", ", overlappingSlabs.Select(s => $"Slab {s.SlabId} ({s.FromDate} to {s.ToDate}, {s.FromKwh}-{s.ToKwh}kWh)"))}"
                    : "This tariff slab overlaps with an existing slab in terms of date range and consumption range for the same tariff type.";

                ModelState.AddModelError("", errorMessage);
                return BadRequest(ModelState);
            }

            await _tariffSlabRepo.UpdateAsync(tariffSlabUpdate);
            return NoContent();
        }

        //[HttpPost("Create")]
        //[ProducesResponseType(201)]
        //[ProducesResponseType(400)]
        //public async Task<ActionResult> CreateTariffSlab([FromBody] TariffSlabDTO model)
        //{
        //    if (model.ToKwh <= model.FromKwh)
        //    {
        //        ModelState.AddModelError("ToKwh", "ToKwh must be greater than FromKwh.");
        //    }

        //    if (model.ToDate <= model.FromDate) 
        //    {
        //        ModelState.AddModelError("ToDate", "ToDate must be greater than FromDate.");
        //    }

        //    if (model == null || !ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var tariffSlabNew = new TariffSlab
        //    {
        //        TariffId = model.TariffId,
        //        FromKwh = model.FromKwh,
        //        ToKwh = model.ToKwh,
        //        RatePerKwh = model.RatePerKwh,
        //        FromDate = DateOnly.FromDateTime(model.FromDate),  
        //        ToDate = DateOnly.FromDateTime(model.ToDate)       
        //    };

        //    await _tariffSlabRepo.AddAsync(tariffSlabNew);
        //    return CreatedAtAction(nameof(GetTariffSlabById), new { id = tariffSlabNew.SlabId }, tariffSlabNew);
        //}

        //[HttpPut("Update")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<ActionResult> UpdateTariffSlab([FromBody] TariffSlabDTO model)
        //{
        //    if (model.ToKwh <= model.FromKwh)
        //    {
        //        ModelState.AddModelError("ToKwh", "ToKwh must be greater than FromKwh.");
        //    }

        //    if (model.ToDate <= model.FromDate)   
        //    {
        //        ModelState.AddModelError("ToDate", "ToDate must be greater than FromDate.");
        //    }

        //    if (model == null || model.SlabId == null || model.SlabId <= 0 || !ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    var existingTariffSlab = await _tariffSlabRepo.GetByIdAsync(model.SlabId.Value);
        //    if (existingTariffSlab == null)
        //    {
        //        return NotFound();
        //    }

        //    var tariffSlabUpdate = new TariffSlab
        //    {
        //        SlabId = model.SlabId.Value,
        //        TariffId = model.TariffId,
        //        FromKwh = model.FromKwh,
        //        ToKwh = model.ToKwh,
        //        RatePerKwh = model.RatePerKwh,
        //        FromDate = DateOnly.FromDateTime(model.FromDate),  
        //        ToDate = DateOnly.FromDateTime(model.ToDate)       
        //    };

        //    await _tariffSlabRepo.UpdateAsync(tariffSlabUpdate);
        //    return NoContent();
        //}

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

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }

    public class TariffSlabDetailDTO
    {
        public int SlabId { get; set; }
        public int TariffId { get; set; }
        public string TariffName { get; set; } = null!;
        public decimal FromKwh { get; set; }
        public decimal ToKwh { get; set; }
        public decimal RatePerKwh { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }
}
