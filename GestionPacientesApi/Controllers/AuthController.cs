using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionPacientesApi.Controllers
{
    // API controller for handling authentication-related endpoints
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Unit of work for accessing repository methods
        private readonly IUnitOfWork _unitOfWork;
        // Configuration for accessing JWT settings
        private readonly IConfiguration _configuration;

        // Constructor for dependency injection of unit of work and configuration
        public AuthController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        // POST endpoint for user login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Retrieve user by matching username and password hash
            var user = (await _unitOfWork.Users.FindAsync(u => u.Username == loginDto.Username && u.PasswordHash == loginDto.Password)).FirstOrDefault();
            // Return Unauthorized if user is not found or credentials are invalid
            if (user == null) return Unauthorized("Invalid credentials.");

            // Generate JWT token for the authenticated user
            var token = GenerateJwtToken(user);
            // Return the token in the response
            return Ok(new { Token = token });
        }

        // Generates a JWT token for the authenticated user
        private string GenerateJwtToken(User user)
        {
            // Define claims for the JWT token, including username, unique ID, and role
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create a symmetric security key from the JWT key in configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            // Create signing credentials using HMAC-SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT token with issuer, audience, claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            // Serialize the token to a string and return it
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Data Transfer Object for user login credentials
    public class LoginDto
    {
        // Username for login, required field
        [Required]
        public string Username { get; set; } = string.Empty;

        // Password for login, required field
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}