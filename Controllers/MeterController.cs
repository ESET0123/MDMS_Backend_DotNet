using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : ControllerBase
    {
        private readonly IMeterRepository _meterRepo;
        private readonly IConsumerRepository _consumerRepo;
        public MeterController(IMeterRepository meterRepo, IConsumerRepository consumerRepo)
        {
            _meterRepo = meterRepo;
            _consumerRepo = consumerRepo;
        }

        [HttpGet("AllMeters")]
        public async Task<ActionResult<IEnumerable<MeterDetailDTO>>> GetAllMeters()
        {
            var meters = await _meterRepo.GetAllAsync();
            var dtos = meters.Select(m => new MeterDetailDTO
            {
                MeterId = m.MeterId,
                Dtrid = m.Dtrid, // ADD THIS LINE
                Ipaddress = m.Ipaddress,
                Firmware = m.Firmware,
                InstallDate = m.InstallDate.ToDateTime(TimeOnly.MinValue),
                LatestReading = m.LatestReading,
                CurrentReading = m.LatestReading,
                LastReadingDate = null,
                ConsumerId = m.ConsumerId,
                ConsumerName = m.Consumer?.Name ?? "Unknown Consumer",
                DtrName = m.Dtr?.Dtrname ?? "Unknown DTR",
                ManufacturerName = m.Manufacturer?.Name ?? "Unknown Manufacturer",
                TariffName = m.Tariff?.Name ?? "Unknown Tariff",
                TariffId = m.TariffId,
                BaseRate = m.Tariff?.BaseRate ?? 0,
                StatusName = m.Status?.Name ?? "Unknown Status",
                Iccid = m.Iccid,
                Imsi = m.Imsi
            });
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
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
                Dtrid = m.Dtrid, // ADD THIS LINE
                Ipaddress = m.Ipaddress,
                Firmware = m.Firmware,
                InstallDate = m.InstallDate.ToDateTime(TimeOnly.MinValue),
                LatestReading = m.LatestReading,
                CurrentReading = m.LatestReading,
                LastReadingDate = null,
                ConsumerId = m.ConsumerId,
                ConsumerName = m.Consumer?.Name ?? "Unknown Consumer",
                DtrName = m.Dtr?.Dtrname ?? "Unknown DTR",
                ManufacturerName = m.Manufacturer?.Name ?? "Unknown Manufacturer",
                TariffName = m.Tariff?.Name ?? "Unknown Tariff",
                TariffId = m.TariffId,
                BaseRate = m.Tariff?.BaseRate ?? 0,
                StatusName = m.Status?.Name ?? "Unknown Status",
                Iccid = m.Iccid,
                Imsi = m.Imsi
            };
            return Ok(dto);
        }

        [HttpGet("{id:int}/LatestReading")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<MeterReadingInfoDTO>> GetLatestReading(int id)
        {
            var meter = await _meterRepo.GetByIdAsync(id);
            if (meter == null) return NotFound();

            var dto = new MeterReadingInfoDTO
            {
                MeterId = id,
                CurrentReading = meter.LatestReading,
                PreviousReading = meter.LatestReading, // For first reading
                LastReadingDate = null,
                LastRecordedBy = null,
                ConsumerName = meter.Consumer?.Name ?? "Unknown",
                TariffRate = meter.Tariff?.BaseRate ?? 0
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
                InstallDate = DateOnly.FromDateTime(model.InstallDate),
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
        // Add these endpoints to your existing MeterController.cs

        [HttpPost("ValidateBulk")]
        [ProducesResponseType(200, Type = typeof(BulkValidationResult))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BulkValidationResult>> ValidateBulkMeters([FromBody] List<MeterDTO> meters)
        {
            var result = new BulkValidationResult
            {
                TotalRecords = meters.Count,
                ValidRecords = new List<MeterDTO>(),
                InvalidRecords = new List<ValidationError>()
            };

            for (int i = 0; i < meters.Count; i++)
            {
                var meter = meters[i];
                var errors = new List<string>();

                // Validate Consumer exists
                var consumer = await _consumerRepo.GetByIdAsync(meter.ConsumerId);
                if (consumer == null)
                {
                    errors.Add($"Consumer ID {meter.ConsumerId} does not exist");
                }

                // Validate DTR exists (you'll need to add this method to your repository)
                // var dtr = await _meterRepo.DtrExistsAsync(meter.Dtrid);
                // if (!dtr) errors.Add($"DTR ID {meter.Dtrid} does not exist");

                // Validate Manufacturer exists
                // var manufacturer = await _meterRepo.ManufacturerExistsAsync(meter.ManufacturerId);
                // if (!manufacturer) errors.Add($"Manufacturer ID {meter.ManufacturerId} does not exist");

                // Validate Tariff exists
                // var tariff = await _meterRepo.TariffExistsAsync(meter.TariffId);
                // if (!tariff) errors.Add($"Tariff ID {meter.TariffId} does not exist");

                // Validate Status exists
                // var status = await _meterRepo.StatusExistsAsync(meter.StatusId);
                // if (!status) errors.Add($"Status ID {meter.StatusId} does not exist");

                // Validate IP Address format
                if (!IsValidIpAddress(meter.Ipaddress))
                {
                    errors.Add("Invalid IP address format");
                }

                // Validate Install Date
                if (meter.InstallDate > DateTime.Now)
                {
                    errors.Add("Install date cannot be in the future");
                }

                // Validate Latest Reading
                if (meter.LatestReading < 0)
                {
                    errors.Add("Latest reading cannot be negative");
                }

                if (errors.Any())
                {
                    result.InvalidRecords.Add(new ValidationError
                    {
                        RowNumber = i + 1,
                        MeterData = meter,
                        Errors = errors
                    });
                }
                else
                {
                    result.ValidRecords.Add(meter);
                }
            }

            result.ValidCount = result.ValidRecords.Count;
            result.InvalidCount = result.InvalidRecords.Count;
            result.IsValid = result.InvalidCount == 0;

            return Ok(result);
        }

        [HttpPost("BulkCreate")]
        [ProducesResponseType(200, Type = typeof(BulkUploadResult))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BulkUploadResult>> BulkCreateMeters([FromBody] List<MeterDTO> meters)
        {
            var result = new BulkUploadResult
            {
                TotalRecords = meters.Count,
                SuccessCount = 0,
                FailedCount = 0,
                FailedRecords = new List<BulkUploadError>()
            };

            for (int i = 0; i < meters.Count; i++)
            {
                try
                {
                    var meter = meters[i];
                    var meterNew = new Meter
                    {
                        ConsumerId = meter.ConsumerId,
                        Dtrid = meter.Dtrid,
                        Ipaddress = meter.Ipaddress,
                        Iccid = meter.Iccid,
                        Imsi = meter.Imsi,
                        ManufacturerId = meter.ManufacturerId,
                        Firmware = meter.Firmware,
                        TariffId = meter.TariffId,
                        InstallDate = DateOnly.FromDateTime(meter.InstallDate),
                        StatusId = meter.StatusId,
                        LatestReading = meter.LatestReading
                    };

                    await _meterRepo.AddAsync(meterNew);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.FailedRecords.Add(new BulkUploadError
                    {
                        RowNumber = i + 1,
                        MeterData = meters[i],
                        ErrorMessage = ex.Message
                    });
                }
            }

            return Ok(result);
        }

        private bool IsValidIpAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0 || num > 255)
                    return false;
            }

            return true;
        }
    }

    // DTOs for validation and bulk upload
    public class BulkValidationResult
    {
        public int TotalRecords { get; set; }
        public int ValidCount { get; set; }
        public int InvalidCount { get; set; }
        public bool IsValid { get; set; }
        public List<MeterDTO> ValidRecords { get; set; } = new List<MeterDTO>();
        public List<ValidationError> InvalidRecords { get; set; } = new List<ValidationError>();
    }

    public class ValidationError
    {
        public int RowNumber { get; set; }
        public MeterDTO MeterData { get; set; } = null!;
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class BulkUploadResult
    {
        public int TotalRecords { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<BulkUploadError> FailedRecords { get; set; } = new List<BulkUploadError>();
    }

    public class BulkUploadError
    {
        public int RowNumber { get; set; }
        public MeterDTO MeterData { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    // ==================== DTOs ====================

    public class MeterDTO
    {
        public int? MeterId { get; set; }

        [Required]
        public int ConsumerId { get; set; }

        [Required]
        public int Dtrid { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 7)]
        public string Ipaddress { get; set; } = null!;

        public string? Iccid { get; set; }

        public string? Imsi { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        public string? Firmware { get; set; }

        [Required]
        public int TariffId { get; set; }

        [Required]
        public DateTime InstallDate { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal LatestReading { get; set; } = 0;
    }

    public class MeterDetailDTO
    {
        public int MeterId { get; set; }
        public int Dtrid { get; set; } // ADD THIS LINE
        public string Ipaddress { get; set; } = null!;
        public string? Firmware { get; set; }
        public DateTime InstallDate { get; set; }
        public decimal LatestReading { get; set; }
        public decimal CurrentReading { get; set; }
        public DateTime? LastReadingDate { get; set; }
        public int ConsumerId { get; set; }
        public string? Iccid { get; set; }
        public string? Imsi { get; set; }
        public string ConsumerName { get; set; } = null!;
        public string DtrName { get; set; } = null!;
        public string ManufacturerName { get; set; } = null!;
        public string TariffName { get; set; } = null!;
        public int TariffId { get; set; }
        public decimal BaseRate { get; set; }
        public string StatusName { get; set; } = null!;
        // Additional info
        public DateTime? LastRecordDate { get; set; }
        public string? LastRecordBy { get; set; }
    }

    public class MeterReadingInfoDTO
    {
        public int MeterId { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal PreviousReading { get; set; }
        public DateTime? LastReadingDate { get; set; }
        public string? LastRecordedBy { get; set; }
        public string ConsumerName { get; set; } = null!;
        public decimal TariffRate { get; set; }
    }
}