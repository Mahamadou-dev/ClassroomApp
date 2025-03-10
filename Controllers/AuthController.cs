using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using BCrypt.Net; // Pour hachage sécurisé
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _secretKey = configuration["Jwt:SecretKey"];
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { Message = "Email et mot de passe sont obligatoires." });

            // Validation du format de l'email
            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { Message = "Format d'email invalide." });
            }

            _logger.LogInformation($"Email: {model.Email}");

            try
            {
                var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);
                _logger.LogInformation($"User found: {user != null}");

                if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.Password))
                {
                    return Unauthorized(new { Message = "Email ou mot de passe incorrect." });
                }

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error occurred while processing the login request.");
                return StatusCode(500, new { Message = "A database error occurred while processing your request." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the login request.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            // Validation des champs obligatoires
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { Message = "Email et mot de passe sont obligatoires." });

            // Validation du format de l'email
            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { Message = "Format d'email invalide." });
            }

            // Validation du mot de passe
            if (model.Password.Length < 8)
                return BadRequest(new { Message = "Le mot de passe doit contenir au moins 8 caractères." });

            if (!Regex.IsMatch(model.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"))
            {
                return BadRequest(new { Message = "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial." });
            }

            // Validation du rôle
            if (!Enum.IsDefined(typeof(RoleUtilisateur), model.Role))
                return BadRequest(new { Message = "Rôle invalide." });

            // Validation du CIN pour les étudiants et enseignants
            if (model.Role == RoleUtilisateur.Etudiant || model.Role == RoleUtilisateur.Enseignant)
            {
                if (string.IsNullOrWhiteSpace(model.CIN))
                    return BadRequest(new { Message = "Le CIN est obligatoire pour les étudiants et enseignants." });
            }

            // Validation de la SuperKey pour les admins
            if (model.Role == RoleUtilisateur.Admin)
            {
                if (string.IsNullOrWhiteSpace(model.SuperKey))
                    return BadRequest(new { Message = "La SuperKey est obligatoire pour les admins." });
            }

            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest(new { Message = "Un utilisateur avec cet email existe déjà." });

            // Hacher le mot de passe
            var hashedPassword = PasswordHasher.HashPassword(model.Password);

            // Créer un nouvel utilisateur en fonction du rôle
            Utilisateur newUser;
            switch (model.Role)
            {
                case RoleUtilisateur.Etudiant:
                    newUser = new Etudiant
                    {
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        Email = model.Email,
                        Password = hashedPassword,
                        Role = model.Role,
                        Identifiant = new Identifiant
                        {
                            CIN = model.CIN,
                            Type = "Etudiant"
                        }
                    };
                    break;

                case RoleUtilisateur.Enseignant:
                    newUser = new Enseignant
                    {
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        Email = model.Email,
                        Password = hashedPassword,
                        Role = model.Role,
                        Identifiant = new Identifiant
                        {
                            CIN = model.CIN,
                            Type = "Enseignant"
                        }
                    };
                    break;

                case RoleUtilisateur.Admin:
                    newUser = new Admin
                    {
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        Email = model.Email,
                        Password = hashedPassword,
                        Role = model.Role,
                        Identifiant = new Identifiant
                        {
                            SuperKey = model.SuperKey,
                            Type = "Admin"
                        }
                    };
                    break;

                default:
                    return BadRequest(new { Message = "Rôle invalide." });
            }

            // Ajouter l'utilisateur à la base de données
            _context.Utilisateurs.Add(newUser);
            await _context.SaveChangesAsync();

            // Générer un token JWT
            var token = GenerateJwtToken(newUser);

            // Retourner le token
            return Ok(new { Token = token });
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
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class UserRegisterDto
    {
        [Required]
        [MaxLength(255)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(255)]
        public string Prenom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        public RoleUtilisateur Role { get; set; } // Enum pour les rôles

        // Identifiant (CIN ou SuperKey)
        public string? CIN { get; set; } // Pour les étudiants et enseignants
        public string? SuperKey { get; set; } // Pour les admins
    }

    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}