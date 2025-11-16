using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo )
        {
            _userRepo = userRepo;
        }

        [HttpGet("AllUsers")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDetailDTO>))]
        public async Task<ActionResult<IEnumerable<UserDetailDTO>>> GetUsers()
        {
            var users = await _userRepo.GetAllAsync();

            var dtos = users.Select(u => new UserDetailDTO
            {
                UserNumber = u.UserNumber,
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                RoleName = u.Role?.RoleName ?? "N/A",
                LastLogin = u.LastLogin,
                Active = u.Active
            });

            return Ok(dtos);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(UserDetailDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailDTO>> GetUserById(string userId)
        {
            var user = await _userRepo.GetByUserIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var dto = new UserDetailDTO
            {
                UserNumber = user.UserNumber,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName ?? "N/A",
                LastLogin = user.LastLogin,
                Active = user.Active
            };

            return Ok(dto);
        }


        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.PasswordString) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var sha256 = SHA256.Create();
            var passwordHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.PasswordString));

            var userNew = new User
            {
                UserId = model.UserId ,
                Username = model.Username,
                Email = model.Email,
                Phone = model.Phone,
                PasswordHashed = passwordHashBytes,
                RoleId = model.RoleId,
                Active = model.Active,
                LastLogin = null
            };

            await _userRepo.AddAsync(userNew);
            return CreatedAtAction(nameof(GetUserById), new { userId = userNew.UserId }, null);
        }

        [HttpPut("Update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateUser([FromBody] UserDTO model)
        {
            if (string.IsNullOrEmpty(model.UserId) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userRepo.GetByUserIdAsync(model.UserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Username = model.Username;
            existingUser.Email = model.Email;
            existingUser.Phone = model.Phone;
            existingUser.RoleId = model.RoleId;
            existingUser.Active = model.Active;

            if (!string.IsNullOrEmpty(model.PasswordString))
            {
                using var sha256 = SHA256.Create();
                existingUser.PasswordHashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.PasswordString));
            }

            await _userRepo.UpdateAsync(existingUser);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var existingUser = await _userRepo.GetByUserIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userRepo.DeleteAsync(userId);
            return NoContent();
        }
    }
    public class UserDTO
    {
        public string? UserId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        public string? PasswordString { get; set; }

        [Required]
        public int RoleId { get; set; }

        public bool Active { get; set; } = true;
    }

    public class UserDetailDTO
    {
        public int UserNumber { get; set; }
        public string UserId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }
    }
}
