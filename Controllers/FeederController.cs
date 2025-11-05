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
    public class FeederController : ControllerBase
    {
        private readonly IFeederRepository _feederRepo;

        public FeederController(IFeederRepository feederRepo)
        {
            _feederRepo = feederRepo;
        }

        [HttpGet("AllFeeders")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FeederDetailDTO>))]
        public async Task<ActionResult<IEnumerable<FeederDetailDTO>>> GetFeeders()
        {
            var feeders = await _feederRepo.GetAllAsync();

            var dtos = feeders.Select(f => new FeederDetailDTO
            {
                FeederId = f.FeederId,
                FeederName = f.FeederName,
                SubstationId = f.SubstationId,
                SubstationName = f.Substation?.SubstationName ?? "Unknown Substation" 
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(FeederDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<FeederDetailDTO>> GetFeederById(int id)
        {
            var feeder = await _feederRepo.GetByIdAsync(id);

            if (feeder == null)
            {
                return NotFound();
            }

            var dto = new FeederDetailDTO
            {
                FeederId = feeder.FeederId,
                FeederName = feeder.FeederName,
                SubstationId = feeder.SubstationId,
                SubstationName = feeder.Substation?.SubstationName ?? "Unknown Substation"
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateFeeder([FromBody] FeederDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feederNew = new Feeder
            {
                FeederName = model.FeederName,
                SubstationId = model.SubstationId
            };

            await _feederRepo.AddAsync(feederNew);
            return CreatedAtAction(nameof(GetFeederById), new { id = feederNew.FeederId }, feederNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateFeeder([FromBody] FeederDTO model)
        {
            if (model == null || model.FeederId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingFeeder = await _feederRepo.GetByIdAsync(model.FeederId);
            if (existingFeeder == null)
            {
                return NotFound();
            }

            var feederUpdate = new Feeder
            {
                FeederId = model.FeederId,
                FeederName = model.FeederName,
                SubstationId = model.SubstationId
            };

            await _feederRepo.UpdateAsync(feederUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteFeeder(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingFeeder = await _feederRepo.GetByIdAsync(id);
            if (existingFeeder == null)
            {
                return NotFound();
            }

            await _feederRepo.DeleteAsync(id);
            return NoContent();
        }
    }
    public class FeederDTO
    {
        public int FeederId { get; set; }

        [Required]
        public string FeederName { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SubstationId must be a positive integer.")]
        public int SubstationId { get; set; }
    }

    public class FeederDetailDTO
    {
        public int FeederId { get; set; }
        public string FeederName { get; set; } = null!;
        public int SubstationId { get; set; }
        public string SubstationName { get; set; } = null!;
    }
}
