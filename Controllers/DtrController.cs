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
    public class DtrController : ControllerBase
    {
        private readonly IDtrRepository _dtrRepo;

        public DtrController(IDtrRepository dtrRepo)
        {
            _dtrRepo = dtrRepo;
        }

        [HttpGet("AllDtrs")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DtrDetailDTO>))]
        public async Task<ActionResult<IEnumerable<DtrDetailDTO>>> GetAllDtrs()
        {
            var dtrs = await _dtrRepo.GetAllAsync();

            var dtos = dtrs.Select(d => new DtrDetailDTO
            {
                Dtrid = d.Dtrid,
                Dtrname = d.Dtrname,
                FeederId = d.FeederId,
                FeederName = d.Feeder?.FeederName ?? "Unknown Feeder"
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(DtrDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DtrDetailDTO>> GetDtrById(int id)
        {
            var dtr = await _dtrRepo.GetByIdAsync(id);

            if (dtr == null)
            {
                return NotFound();
            }

            var dto = new DtrDetailDTO
            {
                Dtrid = dtr.Dtrid,
                Dtrname = dtr.Dtrname,
                FeederId = dtr.FeederId,
                FeederName = dtr.Feeder?.FeederName ?? "Unknown Feeder"
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateDtr([FromBody] DtrDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dtrNew = new Dtr
            {
                Dtrname = model.Dtrname,
                FeederId = model.FeederId
            };

            await _dtrRepo.AddAsync(dtrNew);
            return CreatedAtAction(nameof(GetDtrById), new { id = dtrNew.Dtrid }, dtrNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateDtr([FromBody] DtrDTO model)
        {
            if (model == null || model.Dtrid == null || model.Dtrid <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingDtr = await _dtrRepo.GetByIdAsync(model.Dtrid.Value);
            if (existingDtr == null)
            {
                return NotFound();
            }

            var dtrUpdate = new Dtr
            {
                Dtrid = model.Dtrid.Value,
                Dtrname = model.Dtrname,
                FeederId = model.FeederId
            };

            await _dtrRepo.UpdateAsync(dtrUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteDtr(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingDtr = await _dtrRepo.GetByIdAsync(id);
            if (existingDtr == null)
            {
                return NotFound();
            }

            await _dtrRepo.DeleteAsync(id);
            return NoContent();
        }
    }
    public class DtrDTO
    {
        public int? Dtrid { get; set; } // Nullable for creation

        [Required]
        public string Dtrname { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "FeederId must be a positive integer.")]
        public int FeederId { get; set; }
    }

    // DTO for Read operations
    public class DtrDetailDTO
    {
        public int Dtrid { get; set; }
        public string Dtrname { get; set; } = null!;
        public int FeederId { get; set; }
        public string FeederName { get; set; } = null!;
    }
}
