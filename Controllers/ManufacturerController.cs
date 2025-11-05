using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerRepository _manufacturerRepo;

        public ManufacturerController(IManufacturerRepository manufacturerRepo)
        {
            _manufacturerRepo = manufacturerRepo;
        }

        [HttpGet("AllManufacturers")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ManufacturerDetailDTO>))]
        public async Task<ActionResult<IEnumerable<ManufacturerDetailDTO>>> GetAllManufacturers()
        {
            var manufacturers = await _manufacturerRepo.GetAllAsync();

            var dtos = manufacturers.Select(m => new ManufacturerDetailDTO
            {
                ManufacturerId = m.ManufacturerId,
                Name = m.Name,
                MeterCount = m.Meters?.Count ?? 0
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ManufacturerDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ManufacturerDetailDTO>> GetManufacturerById(int id)
        {
            var manufacturer = await _manufacturerRepo.GetByIdAsync(id);

            if (manufacturer == null)
            {
                return NotFound();
            }

            var dto = new ManufacturerDetailDTO
            {
                ManufacturerId = manufacturer.ManufacturerId,
                Name = manufacturer.Name,
                MeterCount = manufacturer.Meters?.Count ?? 0
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateManufacturer([FromBody] ManufacturerDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var manufacturerNew = new Manufacturer
            {
                Name = model.Name
            };

            await _manufacturerRepo.AddAsync(manufacturerNew);
            return CreatedAtAction(nameof(GetManufacturerById), new { id = manufacturerNew.ManufacturerId }, manufacturerNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateManufacturer([FromBody] ManufacturerDTO model)
        {
            if (model == null || model.ManufacturerId == null || model.ManufacturerId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingManufacturer = await _manufacturerRepo.GetByIdAsync(model.ManufacturerId.Value);
            if (existingManufacturer == null)
            {
                return NotFound();
            }

            var manufacturerUpdate = new Manufacturer
            {
                ManufacturerId = model.ManufacturerId.Value,
                Name = model.Name
            };

            await _manufacturerRepo.UpdateAsync(manufacturerUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteManufacturer(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingManufacturer = await _manufacturerRepo.GetByIdAsync(id);
            if (existingManufacturer == null)
            {
                return NotFound();
            }

            await _manufacturerRepo.DeleteAsync(id);
            return NoContent();
        }
    }

    public class ManufacturerDTO
    {
        public int? ManufacturerId { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
    }

    public class ManufacturerDetailDTO
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; } = null!;
        public int MeterCount { get; set; }
    }
}