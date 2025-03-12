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
        private readonly AppDbContext _context;
        private readonly string _secretKey;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is missing.");
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Tentative de connexion avec des champs vides.");
                return BadRequest(new { Message = "Email et mot de passe sont obligatoires." });
            }

            // Validation du format de l'email
            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _logger.LogWarning($"Format d'email invalide : {model.Email}");
                return BadRequest(new { Message = "Format d'email invalide." });
            }

            _logger.LogInformation($"Tentative de connexion pour l'email : {model.Email}");

            try
            {
                // Rechercher l'utilisateur par email
                _logger.LogInformation("Recherche de l'utilisateur dans la base de données...");
                var user = await _context.Utilisateurs
                    .Include(u => u.Identifiant) // Charger l'identifiant associé
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    _logger.LogWarning($"Aucun utilisateur trouvé avec l'email : {model.Email}");
                    return Unauthorized(new { Message = "Email ou mot de passe incorrect." });
                }

                _logger.LogInformation("Utilisateur trouvé. Vérification du mot de passe...");

                // Vérifier le mot de passe
                if (!PasswordHasher.VerifyPassword(model.Password, user.Password))
                {
                    _logger.LogWarning("Mot de passe incorrect.");
                    return Unauthorized(new { Message = "Email ou mot de passe incorrect." });
                }

                _logger.LogInformation("Mot de passe vérifié. Génération du token JWT...");

                // Générer un token JWT
                var token = GenerateJwtToken(user);

                _logger.LogInformation("Token généré avec succès.");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la tentative de connexion.");
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la connexion." });
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

            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest(new { Message = "Un utilisateur avec cet email existe déjà." });

            // Vérifier l'identifiant (CIN ou SuperKey) en fonction du rôle
            if (model.Role == RoleUtilisateur.Etudiant || model.Role == RoleUtilisateur.Enseignant)
            {
                if (string.IsNullOrWhiteSpace(model.CIN))
                    return BadRequest(new { Message = "Le CIN est obligatoire pour les étudiants et enseignants." });

                // Vérifier que le CIN existe et n'est pas déjà utilisé
                var identifiant = await _context.Identifiants
                    .FirstOrDefaultAsync(i => i.CIN == model.CIN && !i.EstUtilise);

                if (identifiant == null)
                    return BadRequest(new { Message = "CIN invalide ou déjà utilisé." });
            }
            else if (model.Role == RoleUtilisateur.Admin)
            {
                if (string.IsNullOrWhiteSpace(model.SuperKey))
                    return BadRequest(new { Message = "La SuperKey est obligatoire pour les admins." });

                // Vérifier que la SuperKey existe et n'est pas déjà utilisée
                var identifiant = await _context.Identifiants
                    .FirstOrDefaultAsync(i => i.SuperKey == model.SuperKey && !i.EstUtilise);

                if (identifiant == null)
                    return BadRequest(new { Message = "SuperKey invalide ou déjà utilisée." });
            }

            // Hacher le mot de passe
            var hashedPassword = PasswordHasher.HashPassword(model.Password);

            // Créer un nouvel utilisateur en fonction du rôle
            Utilisateur newUser;
            switch (model.Role)
            {
                case RoleUtilisateur.Etudiant:
                    // Normaliser le nom de la classe (insensible à la casse)
                    var classe = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Nom.ToUpper() == model.ClasseNom.ToUpper());

                    if (classe == null)
                        return BadRequest(new { Message = "La classe spécifiée n'existe pas." });

                    newUser = new Etudiant
                    {
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        Email = model.Email,
                        Password = hashedPassword,
                        Role = model.Role,
                        Identifiant = await _context.Identifiants
                            .FirstOrDefaultAsync(i => i.CIN == model.CIN && !i.EstUtilise),
                        ClasseId = classe.Id // Associer l'étudiant à la classe trouvée
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
                        Identifiant = await _context.Identifiants
                            .FirstOrDefaultAsync(i => i.CIN == model.CIN && !i.EstUtilise)
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
                        Identifiant = await _context.Identifiants
                            .FirstOrDefaultAsync(i => i.SuperKey == model.SuperKey && !i.EstUtilise)
                    };
                    break;

                default:
                    return BadRequest(new { Message = "Rôle invalide." });
            }

            // Marquer l'identifiant comme utilisé
            newUser.Identifiant.EstUtilise = true;

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
                Expires = DateTime.UtcNow.AddHours(1), // Token valide pendant 1 heure
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
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public RoleUtilisateur Role { get; set; } // Enum pour les rôles

        // Identifiant (CIN ou SuperKey)
        public string? CIN { get; set; } // Pour les étudiants et enseignants
        public string? SuperKey { get; set; } // Pour les admins

        // Champ pour spécifier le nom de la classe (uniquement pour les étudiants)
        public string? ClasseNom { get; set; }
    }

    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
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