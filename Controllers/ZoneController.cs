using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] 
    public class ZoneController : ControllerBase
    {
        private readonly IZoneRepository _zoneRepo;

        public ZoneController(IZoneRepository zoneRepo)
        {
            _zoneRepo = zoneRepo;
        }

        [HttpGet("AllZones")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Zone>))]
        // [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<Zone>>> GetZones()
        {
            var zones = await _zoneRepo.GetAllAsync();
            return Ok(zones);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(Zone))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Zone>> GetZoneById(int id)
        {
            var zone = await _zoneRepo.GetByIdAsync(id);
            if (zone == null)
            {
                return NotFound();
            }
            return Ok(zone);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateZone([FromBody] ZoneDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var zoneNew = new Zone
            {
                ZoneName = model.ZoneName
            };

            await _zoneRepo.AddAsync(zoneNew);
            // Return 201 Created status
            return CreatedAtAction(nameof(GetZoneById), new { id = zoneNew.ZoneId }, zoneNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)] // No content returned for a successful update
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateZone([FromBody] ZoneDTO model)
        {
            if (model == null || model.ZoneId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingZone = await _zoneRepo.GetByIdAsync(model.ZoneId);
            if (existingZone == null)
            {
                return NotFound();
            }

            var zoneUpdate = new Zone
            {
                ZoneId = model.ZoneId, // Must include ID for the repo update logic
                ZoneName = model.ZoneName
            };

            await _zoneRepo.UpdateAsync(zoneUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteZone(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingZone = await _zoneRepo.GetByIdAsync(id);
            if (existingZone == null)
            {
                return NotFound();
            }

            await _zoneRepo.DeleteAsync(id);
            return NoContent();
        }
    }
    public class ZoneDTO
    {
        // RoleId was implicit in RoleDTO for updates, making it explicit here for clarity
        public int ZoneId { get; set; }

        [Required]
        public string ZoneName { get; set; } = null!;
    }
}
