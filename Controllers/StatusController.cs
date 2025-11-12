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
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly IStatusRepository _statusRepo;

        public StatusController(IStatusRepository statusRepo)
        {
            _statusRepo = statusRepo;
        }

        [HttpGet("AllStatuses")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StatusDetailDTO>))]
        public async Task<ActionResult<IEnumerable<StatusDetailDTO>>> GetStatuses()
        {
            var statuses = await _statusRepo.GetAllAsync();

            var dtos = statuses.Select(s => new StatusDetailDTO
            {
                StatusId = s.StatusId,
                Name = s.Name,
                ConsumerCount = s.Consumers?.Count ?? 0,
                MeterCount = s.Meters?.Count ?? 0
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(StatusDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<StatusDetailDTO>> GetStatusById(int id)
        {
            var status = await _statusRepo.GetByIdAsync(id);

            if (status == null)
            {
                return NotFound();
            }

            var dto = new StatusDetailDTO
            {
                StatusId = status.StatusId,
                Name = status.Name,
                ConsumerCount = status.Consumers?.Count ?? 0,
                MeterCount = status.Meters?.Count ?? 0
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateStatus([FromBody] StatusDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newStatus = new Status
            {
                Name = model.Name
            };

            await _statusRepo.AddAsync(newStatus);
            return CreatedAtAction(nameof(GetStatusById), new { id = newStatus.StatusId }, newStatus);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateStatus([FromBody] StatusDTO model)
        {
            if (model == null || model.StatusId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingStatus = await _statusRepo.GetByIdAsync(model.StatusId);
            if (existingStatus == null)
            {
                return NotFound();
            }

            var statusUpdate = new Status
            {
                StatusId = model.StatusId,
                Name = model.Name
            };

            await _statusRepo.UpdateAsync(statusUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteStatus(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingStatus = await _statusRepo.GetByIdAsync(id);
            if (existingStatus == null)
            {
                return NotFound();
            }

            await _statusRepo.DeleteAsync(id);
            return NoContent();
        }
    }

    public class StatusDTO
    {
        public int StatusId { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }

    public class StatusDetailDTO
    {
        public int StatusId { get; set; }
        public string Name { get; set; } = null!;
        public int ConsumerCount { get; set; }
        public int MeterCount { get; set; }
    }
}
