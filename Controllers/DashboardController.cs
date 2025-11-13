using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DashboardController : ControllerBase
    {
        private readonly MdmsDbContext _dbContext;

        public DashboardController(MdmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Statistics")]
        [ProducesResponseType(200, Type = typeof(DashboardStatisticsDTO))]
        public async Task<ActionResult<DashboardStatisticsDTO>> GetDashboardStatistics()
        {
            try
            {
                var statistics = new DashboardStatisticsDTO
                {
                    TotalMeters = await _dbContext.Meters.CountAsync(),
                    ActiveUsers = await _dbContext.Users.CountAsync(u => u.Active),
                    //TotalOrgUnits = await _dbContext.OrgUnits.CountAsync(),
                    TotalTariffs = await _dbContext.Tariffs.CountAsync(),
                    TotalConsumers = await _dbContext.Consumers.CountAsync(),
                    ActiveConsumers = await _dbContext.Consumers.CountAsync(c => c.StatusId == 1),
                    TotalManufacturers = await _dbContext.Manufacturers.CountAsync(),
                    TotalDtrs = await _dbContext.Dtrs.CountAsync()
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard statistics", error = ex.Message });
            }
        }

        [HttpGet("RecentActivity")]
        [ProducesResponseType(200, Type = typeof(DashboardActivityDTO))]
        public async Task<ActionResult<DashboardActivityDTO>> GetRecentActivity()
        {
            try
            {
                var activity = new DashboardActivityDTO
                {
                    RecentConsumers = await _dbContext.Consumers
                        .OrderByDescending(c => c.CreatedAt)
                        .Take(5)
                        .Select(c => new RecentConsumerDTO
                        {
                            Name = c.Name,
                            Email = c.Email,
                            CreatedAt = c.CreatedAt
                        })
                        .ToListAsync(),

                    RecentUsers = await _dbContext.Users
                        .OrderByDescending(u => u.UserNumber)
                        .Take(5)
                        .Select(u => new RecentUserDTO
                        {
                            Username = u.Username,
                            Email = u.Email,
                            RoleName = u.Role.RoleName
                        })
                        .ToListAsync()
                };

                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving recent activity", error = ex.Message });
            }
        }
    }

    public class DashboardStatisticsDTO
    {
        public int TotalMeters { get; set; }
        public int ActiveUsers { get; set; }
        //public int TotalOrgUnits { get; set; }
        public int TotalTariffs { get; set; }
        public int TotalConsumers { get; set; }
        public int ActiveConsumers { get; set; }
        public int TotalManufacturers { get; set; }
        public int TotalDtrs { get; set; }
    }

    public class DashboardActivityDTO
    {
        public List<RecentConsumerDTO> RecentConsumers { get; set; } = new();
        public List<RecentUserDTO> RecentUsers { get; set; } = new();
    }

    public class RecentConsumerDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class RecentUserDTO
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}