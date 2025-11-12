using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubstationController : ControllerBase
    {
        private readonly ISubstationRepository _substationRepo;

        public SubstationController(ISubstationRepository substationRepo)
        {
            _substationRepo = substationRepo;
        }

        [HttpGet("AllSubstations")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<SubstationDetailDTO>))]
        public async Task<ActionResult<IEnumerable<SubstationDetailDTO>>> GetSubstations()
        {
            var substations = await _substationRepo.GetAllAsync();

            var dtos = substations.Select(s => new SubstationDetailDTO
            {
                SubstationId = s.SubstationId,
                SubstationName = s.SubstationName,
                ZoneId = s.ZoneId,
                ZoneName = s.Zone?.ZoneName ?? "Unknown Zone" // Map related data
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(SubstationDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SubstationDetailDTO>> GetSubstationById(int id)
        {
            var substation = await _substationRepo.GetByIdAsync(id);

            if (substation == null)
            {
                return NotFound();
            }

            var dto = new SubstationDetailDTO
            {
                SubstationId = substation.SubstationId,
                SubstationName = substation.SubstationName,
                ZoneId = substation.ZoneId,
                ZoneName = substation.Zone?.ZoneName ?? "Unknown Zone"
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateSubstation([FromBody] SubstationDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var substationNew = new Substation
            {
                SubstationName = model.SubstationName,
                ZoneId = model.ZoneId
            };

            await _substationRepo.AddAsync(substationNew);
            return CreatedAtAction(nameof(GetSubstationById), new { id = substationNew.SubstationId }, substationNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateSubstation([FromBody] SubstationDTO model)
        {
            if (model == null || model.SubstationId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingSubstation = await _substationRepo.GetByIdAsync(model.SubstationId);
            if (existingSubstation == null)
            {
                return NotFound();
            }

            var substationUpdate = new Substation
            {
                SubstationId = model.SubstationId,
                SubstationName = model.SubstationName,
                ZoneId = model.ZoneId
            };

            await _substationRepo.UpdateAsync(substationUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteSubstation(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingSubstation = await _substationRepo.GetByIdAsync(id);
            if (existingSubstation == null)
            {
                return NotFound();
            }

            await _substationRepo.DeleteAsync(id);
            return NoContent();
        }
    }

    public class SubstationDetailDTO
    {
        public int SubstationId { get; set; }
        public string SubstationName { get; set; } = null!;
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = null!;
    }
}
