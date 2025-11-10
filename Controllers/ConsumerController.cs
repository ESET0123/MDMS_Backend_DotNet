using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ConsumerController : ControllerBase
    {
        private readonly IConsumerRepository _consumerRepo;

        public ConsumerController(IConsumerRepository consumerRepo)
        {
            _consumerRepo = consumerRepo;
        }

        [HttpGet("AllConsumers")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ConsumerDetailDTO>))]
        public async Task<ActionResult<IEnumerable<ConsumerDetailDTO>>> GetConsumers()
        {
            var consumers = await _consumerRepo.GetAllAsync();

            var dtos = consumers.Select(c => new ConsumerDetailDTO
            {
                ConsumerId = c.ConsumerId,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                Email = c.Email,
                StatusId = c.StatusId,
                StatusName = c.Status?.Name ?? "N/A",
                MeterCount = c.Meters?.Count ?? 0,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy
            });

            return Ok(dtos);
        }

        [HttpGet("{consumerId}")]
        [ProducesResponseType(200, Type = typeof(ConsumerDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ConsumerDetailDTO>> GetConsumerById(int consumerId)
        {
            var consumer = await _consumerRepo.GetByIdAsync(consumerId);
            if (consumer == null)
            {
                return NotFound();
            }

            var dto = new ConsumerDetailDTO
            {
                ConsumerId = consumer.ConsumerId,
                Name = consumer.Name,
                Address = consumer.Address,
                Phone = consumer.Phone,
                Email = consumer.Email,
                StatusId = consumer.StatusId,
                StatusName = consumer.Status?.Name ?? "N/A",
                MeterCount = consumer.Meters?.Count ?? 0,
                CreatedAt = consumer.CreatedAt,
                CreatedBy = consumer.CreatedBy,
                UpdatedAt = consumer.UpdatedAt,
                UpdatedBy = consumer.UpdatedBy
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateConsumer([FromBody] ConsumerDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newConsumer = new Consumer
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                StatusId = model.StatusId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = model.CreatedBy,
                UpdatedAt = null,
                UpdatedBy = null
            };

            await _consumerRepo.AddAsync(newConsumer);
            return CreatedAtAction(nameof(GetConsumerById), new { consumerId = newConsumer.ConsumerId }, null);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateConsumer([FromBody] ConsumerDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _consumerRepo.GetByIdAsync(model.ConsumerId);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = model.Name;
            existing.Address = model.Address;
            existing.Phone = model.Phone;
            existing.Email = model.Email;
            existing.StatusId = model.StatusId;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = model.UpdatedBy;

            await _consumerRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{consumerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteConsumer(int consumerId)
        {
            var existing = await _consumerRepo.GetByIdAsync(consumerId);
            if (existing == null)
            {
                return NotFound();
            }

            await _consumerRepo.DeleteAsync(consumerId);
            return NoContent();
        }
    }

    public class ConsumerDTO
    {
        public int ConsumerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public int StatusId { get; set; }

        [Required]
        public string CreatedBy { get; set; } = null!;

        public string? UpdatedBy { get; set; }
    }

    public class ConsumerDetailDTO
    {
        public int ConsumerId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public int MeterCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}




        //// GET: ConsumerView
        //public async Task<IActionResult> Index()
        //{
        //    var consumers = await _consumerRepo.GetAllAsync();

        //    var dtos = consumers.Select(c => new ConsumerDetailDTO
        //    {
        //        ConsumerId = c.ConsumerId,
        //        Name = c.Name,
        //        Address = c.Address,
        //        Phone = c.Phone,
        //        Email = c.Email,
        //        StatusId = c.StatusId,
        //        StatusName = c.Status?.StatusName ?? "N/A",
        //        MeterCount = c.Meters?.Count ?? 0,
        //        CreatedAt = c.CreatedAt,
        //        CreatedBy = c.CreatedBy,
        //        UpdatedAt = c.UpdatedAt,
        //        UpdatedBy = c.UpdatedBy
        //    }).ToList();

        //    return View(dtos);
        //}