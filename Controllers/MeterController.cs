using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] 
    public class MeterController : ControllerBase
    {
        private readonly IMeterRepository _meterRepo;

        public MeterController(IMeterRepository meterRepo)
        {
            _meterRepo = meterRepo;
        }

        [HttpGet("AllMeters")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MeterDetailDTO>))]
        public async Task<ActionResult<IEnumerable<MeterDetailDTO>>> GetAllMeters()
        {
            var meters = await _meterRepo.GetAllAsync();

            var dtos = meters.Select(m => new MeterDetailDTO
            {
                MeterId = m.MeterId,
                Ipaddress = m.Ipaddress,
                Firmware = m.Firmware,
                InstallDate = m.InstallDate.ToDateTime(TimeOnly.MinValue), // Convert DateOnly back to DateTime for DTO
                LatestReading = m.LatestReading,
                ConsumerId = m.ConsumerId,
                // These assume your related models have a 'Name' property
                ConsumerName = m.Consumer?.Name ?? "Unknown Consumer",
                DtrName = m.Dtr?.Dtrname ?? "Unknown DTR",
                ManufacturerName = m.Manufacturer?.Name ?? "Unknown Manufacturer",
                TariffName = m.Tariff?.Name ?? "Unknown Tariff",
                StatusName = m.Status?.Name ?? "Unknown Status"
            });

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(MeterDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MeterDetailDTO>> GetMeterById(int id)
        {
            var m = await _meterRepo.GetByIdAsync(id);

            if (m == null)
            {
                return NotFound();
            }

            var dto = new MeterDetailDTO
            {
                MeterId = m.MeterId,
                Ipaddress = m.Ipaddress,
                Firmware = m.Firmware,
                InstallDate = m.InstallDate.ToDateTime(TimeOnly.MinValue),
                LatestReading = m.LatestReading,
                ConsumerId = m.ConsumerId,
                ConsumerName = m.Consumer?.Name ?? "Unknown Consumer",
                DtrName = m.Dtr?.Dtrname ?? "Unknown DTR",
                ManufacturerName = m.Manufacturer?.Name ?? "Unknown Manufacturer",
                TariffName = m.Tariff?.Name ?? "Unknown Tariff",
                StatusName = m.Status?.Name ?? "Unknown Status"
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateMeter([FromBody] MeterDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meterNew = new Meter
            {
                ConsumerId = model.ConsumerId,
                Dtrid = model.Dtrid,
                Ipaddress = model.Ipaddress,
                Iccid = model.Iccid,
                Imsi = model.Imsi,
                ManufacturerId = model.ManufacturerId,
                Firmware = model.Firmware,
                TariffId = model.TariffId,
                InstallDate = DateOnly.FromDateTime(model.InstallDate), // Convert DateTime from DTO to DateOnly for Model
                StatusId = model.StatusId,
                LatestReading = model.LatestReading
            };

            await _meterRepo.AddAsync(meterNew);
            return CreatedAtAction(nameof(GetMeterById), new { id = meterNew.MeterId }, meterNew);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateMeter([FromBody] MeterDTO model)
        {
            if (model == null || model.MeterId == null || model.MeterId <= 0 || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingMeter = await _meterRepo.GetByIdAsync(model.MeterId.Value);
            if (existingMeter == null)
            {
                return NotFound();
            }

            var meterUpdate = new Meter
            {
                MeterId = model.MeterId.Value,
                ConsumerId = model.ConsumerId,
                Dtrid = model.Dtrid,
                Ipaddress = model.Ipaddress,
                Iccid = model.Iccid,
                Imsi = model.Imsi,
                ManufacturerId = model.ManufacturerId,
                Firmware = model.Firmware,
                TariffId = model.TariffId,
                InstallDate = DateOnly.FromDateTime(model.InstallDate),
                StatusId = model.StatusId,
                LatestReading = model.LatestReading
            };

            await _meterRepo.UpdateAsync(meterUpdate);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteMeter(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingMeter = await _meterRepo.GetByIdAsync(id);
            if (existingMeter == null)
            {
                return NotFound();
            }

            await _meterRepo.DeleteAsync(id);
            return NoContent();
        }
    }
    public class MeterDTO
    {
        public int? MeterId { get; set; } // Nullable for creation

        [Required]
        public int ConsumerId { get; set; }

        [Required]
        public int Dtrid { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 7)] // Basic IP validation might use RegEx in reality
        public string Ipaddress { get; set; } = null!;

        public string? Iccid { get; set; }

        public string? Imsi { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        public string? Firmware { get; set; }

        [Required]
        public int TariffId { get; set; }

        [Required]
        public DateTime InstallDate { get; set; } // Using DateTime for JSON serialization

        [Required]
        public int StatusId { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal LatestReading { get; set; } = 0;
    }

    // DTO for Read operations
    public class MeterDetailDTO
    {
        public int MeterId { get; set; }
        public string Ipaddress { get; set; } = null!;
        public string? Firmware { get; set; }
        public DateTime InstallDate { get; set; }
        public decimal LatestReading { get; set; }

        // Related Names for display
        public int ConsumerId { get; set; }
        public string ConsumerName { get; set; } = null!; // Assuming Consumer model has a Name field
        public string DtrName { get; set; } = null!;
        public string ManufacturerName { get; set; } = null!;
        public string TariffName { get; set; } = null!;
        public string StatusName { get; set; } = null!;
    }
}
