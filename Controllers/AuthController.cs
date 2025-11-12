using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MDMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IConsumerRepository _consumerRepository;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, IConsumerRepository consumerRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _consumerRepository = consumerRepository;
        }

        [HttpPost("Userlogin")]
        public async Task<IActionResult> UserLogin([FromBody] LoginModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Invalid login request");

            var existingUser = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (existingUser == null)
                return Unauthorized("User not found");

            using var sha256 = SHA256.Create();
            var enteredPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            bool isPasswordValid = enteredPasswordHash.SequenceEqual(existingUser.PasswordHashed);

            if (!isPasswordValid)
                return Unauthorized("Invalid credentials");

            existingUser.LastLogin = DateTime.Now;
            await _userRepository.UpdateAsync(existingUser);

            var token = GenerateJwtToken(existingUser.Username, "admin", existingUser.UserId.ToString());

            return Ok(new
            {
                token,
                userType = "admin",
                userId = existingUser.UserId,
                username = existingUser.Username
            });
            //return Ok(new { token });
        }

        [HttpPost("ConsumerLogin")]
        public async Task<IActionResult> ConsumerLogin([FromBody] ConsumerLoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Invalid login request");

            var consumer = await _consumerRepository.GetByEmailAsync(model.Email);
            if (consumer == null)
                return Unauthorized("Consumer not found");

            // Check if consumer account is active
            if (consumer.Status?.Name?.ToLower() != "active")
                return Unauthorized("Account is not active");

            // Verify password
            if (consumer.PasswordHash == null && consumer.PasswordHash.Length == 0)
                return Unauthorized("Password not set for this account");

            using var sha256 = SHA256.Create();
            var passwordHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            bool isPasswordValid = passwordHashBytes.SequenceEqual(consumer.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized("Invalid credentials");

            //if (consumer.PasswordHash != passwordHashBytes)
            //    return Unauthorized($"Invalid credentials {passwordHashBytes}- {consumer.PasswordHash}");

            var token = GenerateJwtToken(consumer.Email, "consumer", consumer.ConsumerId.ToString());

            return Ok(new
            {
                token,
                userType = "consumer",
                consumerId = consumer.ConsumerId,
                name = consumer.Name,
                email = consumer.Email
            });
            //return Ok(new { token });
        }

        private string GenerateJwtToken(string identifier, string role, string userId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, identifier),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ConsumerLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}