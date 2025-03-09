using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using BCrypt;// Ajout de BCrypt

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration["Jwt:SecretKey"];
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email and password are required.");
            }

            try
            {
                var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                if (!BCrypt.Verify(model.Password, user.Password))
                {
                    return Unauthorized("Invalid email or password.");
                }

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Journalisation de l'erreur (à implémenter)
                return StatusCode(500, "Internal server error.");
            }
        }
        private string GenerateJwtToken(Utilisateur user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Conversion en chaîne
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
