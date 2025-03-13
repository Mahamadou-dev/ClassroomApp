
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BCrypt.Net;
using global::Backend.Data;
using global::Backend.Models;
using System.ComponentModel.DataAnnotations;
namespace Backend.Controllers
{
   

        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private readonly AppDbContext _context;

            public UsersController(AppDbContext context)
            {
                _context = context;
            }

        // GET: api/users/admin
        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<AdminDto>> GetAdminInfo()
        {
            try
            {
                // Récupérer l'ID de l'admin depuis le token
                var adminId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized(new { Message = "Admin ID non trouvé dans le token." });
                }

                if (!int.TryParse(adminId, out int adminIdInt))
                {
                    return BadRequest(new { Message = "Admin ID invalide." });
                }

                // Récupérer l'admin depuis la base de données
                var admin = await _context.Admins
                    .Include(a => a.Identifiant) // Inclure l'identifiant si nécessaire
                    .FirstOrDefaultAsync(a => a.Id == adminIdInt);

                if (admin == null)
                {
                    return NotFound(new { Message = "Admin non trouvé." });
                }

                // Mapper l'entité Admin vers AdminDto
                var adminDto = new AdminDto
                {
                    Id = admin.Id,
                    Nom = admin.Nom,
                    Prenom = admin.Prenom,
                    Email = admin.Email,
                    Role = admin.Role.ToString(), // Convertir l'enum en string si nécessaire
                    SuperKey = admin.Identifiant?.SuperKey // Inclure le CIN si l'identifiant est chargé
                };

                return adminDto;
            }
            catch (Exception ex)
            {
                // Log l'exception
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        // GET: api/users/enseignants
        [HttpGet("enseignants")]
            [Authorize(Policy = "AdminAndEnseignant")]
            public async Task<ActionResult<IEnumerable<EnseignantResponseDto>>> GetEnseignants()
            {
                var enseignants = await _context.Enseignants
                    .Include(e => e.Cours)
                    .Select(e => new EnseignantResponseDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        Prenom = e.Prenom,
                        Email = e.Email,
                        SuperKey = e.Identifiant.SuperKey,
                        CoursCrees = e.Cours.Select(c => c.Titre).ToList()
                    })
                    .ToListAsync();

                return enseignants;
            }

            // GET: api/users/etudiants
            [HttpGet("etudiants")]
            [Authorize(Policy = "AdminAndEnseignant")]
            public async Task<ActionResult<IEnumerable<EtudiantResponseDto>>> GetEtudiants()
            {
                var etudiants = await _context.Etudiants
                    .Include(e => e.Identifiant)
                    .Include(e => e.Classe)
                    .Select(e => new EtudiantResponseDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        Prenom = e.Prenom,
                        Email = e.Email,
                        CIN = e.Identifiant.CIN,
                        NomClasse = e.Classe.Nom
                    })
                    .ToListAsync();

                return etudiants;
            }

            // GET: api/users/etudiants/classe/{nomClasse}
            [HttpGet("etudiants/classe/{nomClasse}")]
            [Authorize]
            public async Task<ActionResult<IEnumerable<EtudiantResponseDto>>> GetEtudiantsByClasse(string nomClasse)
            {
                var etudiants = await _context.Etudiants
                    .Include(e => e.Identifiant)
                    .Include(e => e.Classe)
                    .Where(e => e.Classe.Nom.ToUpper() == nomClasse.ToUpper())
                    .Select(e => new EtudiantResponseDto
                    {
                        Id = e.Id,
                        Nom = e.Nom,
                        Prenom = e.Prenom,
                        Email = e.Email,
                        CIN = e.Identifiant.CIN,
                        NomClasse = e.Classe.Nom
                    })
                    .ToListAsync();

                return etudiants;
            }

        [HttpPost("enseignants")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<EnseignantResponseDto>> PostEnseignant(CreateEnseignantDto enseignantDto)
        {
            // Vérifier que le SuperKey existe et n'est pas déjà utilisé
            var identifiant = await _context.Identifiants
                .FirstOrDefaultAsync(i => i.SuperKey == enseignantDto.SuperKey && !i.EstUtilise);

            if (identifiant == null)
            {
                return BadRequest(new { Message = "Superkey invalide ou déjà utilisé." });
            }

            // Vérifier si un enseignant avec le même email existe déjà
            var enseignantExistant = await _context.Utilisateurs
                .OfType<Enseignant>()
                .FirstOrDefaultAsync(u => u.Email == enseignantDto.Email);

            if (enseignantExistant != null)
            {
                return Conflict(new { Message = "Un enseignant avec cet email existe déjà." });
            }

            // Créer l'entité Enseignant
            var enseignant = new Enseignant
            {
                Nom = enseignantDto.Nom,
                Prenom = enseignantDto.Prenom,
                Email = enseignantDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(enseignantDto.Password),
                Role = RoleUtilisateur.Enseignant,
                Identifiant = identifiant
            };

            _context.Utilisateurs.Add(enseignant);
            await _context.SaveChangesAsync();

            // Mapper l'entité Enseignant vers EnseignantResponseDto
            var enseignantResponse = new EnseignantResponseDto2
            {
                Id = enseignant.Id,
                Nom = enseignant.Nom,
                Prenom = enseignant.Prenom,
                Email = enseignant.Email,
                Role = enseignant.Role.ToString(),
                SuperKey = enseignant.Identifiant.SuperKey
            };

            return CreatedAtAction(nameof(GetEnseignants), new { id = enseignant.Id }, enseignantResponse);
        }
        // POST: api/users/etudiants
        [HttpPost("etudiants")]
            [Authorize(Policy = "AdminOnly")]
            public async Task<ActionResult<Etudiant>> PostEtudiant(CreateEtudiantDto etudiantDto)
            {
                // Vérifier que le CIN existe et n'est pas déjà utilisé
                var identifiant = await _context.Identifiants
                    .FirstOrDefaultAsync(i => i.CIN == etudiantDto.CIN && !i.EstUtilise);

                if (identifiant == null)
                {
                    return BadRequest(new { Message = "CIN invalide ou déjà utilisé." });
                }

                // Vérifier que la classe existe
                var classe = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Nom.ToUpper() == etudiantDto.NomClasse.ToUpper());

                if (classe == null)
                {
                    return BadRequest(new { Message = "Classe non trouvée." });
                }

                var etudiant = new Etudiant
                {
                    Nom = etudiantDto.Nom,
                    Prenom = etudiantDto.Prenom,
                    Email = etudiantDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(etudiantDto.Password),
                    Role = RoleUtilisateur.Etudiant,
                    Identifiant = identifiant,
                    ClasseId = classe.Id
                };

                _context.Utilisateurs.Add(etudiant);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEtudiants), new { id = etudiant.Id }, etudiant);
            }

            // PUT: api/users/admin
            [HttpPut("admin")]
            [Authorize(Policy = "AdminOnly")]
            public async Task<IActionResult> PutAdminInfo(UpdateUserDto updateDto)
            {
                var adminId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized(new { Message = "Admin ID non trouvé dans le token." });
                }

                var admin = await _context.Admins.FindAsync(int.Parse(adminId));
                if (admin == null)
                {
                    return NotFound(new { Message = "Admin non trouvé." });
                }

                // Mettre à jour les informations
                admin.Nom = updateDto.Nom ?? admin.Nom;
                admin.Prenom = updateDto.Prenom ?? admin.Prenom;
                admin.Email = updateDto.Email ?? admin.Email;
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    admin.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }

                _context.Entry(admin).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // PUT: api/users/enseignants
            [HttpPut("enseignants")]
            [Authorize(Policy = "EnseignantOnly")]
            public async Task<IActionResult> PutEnseignantInfo(UpdateUserDto updateDto)
            {
                var enseignantId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(enseignantId))
                {
                    return Unauthorized(new { Message = "Enseignant ID non trouvé dans le token." });
                }

                var enseignant = await _context.Enseignants.FindAsync(int.Parse(enseignantId));
                if (enseignant == null)
                {
                    return NotFound(new { Message = "Enseignant non trouvé." });
                }

                // Mettre à jour les informations
                enseignant.Nom = updateDto.Nom ?? enseignant.Nom;
                enseignant.Prenom = updateDto.Prenom ?? enseignant.Prenom;
                enseignant.Email = updateDto.Email ?? enseignant.Email;
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    enseignant.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }

                _context.Entry(enseignant).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // PUT: api/users/etudiants
            [HttpPut("etudiants")]
            [Authorize(Policy = "EtudiantOnly")]
            public async Task<IActionResult> PutEtudiantInfo(UpdateUserDto updateDto)
            {
                var etudiantId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(etudiantId))
                {
                    return Unauthorized(new { Message = "Étudiant ID non trouvé dans le token." });
                }

                var etudiant = await _context.Etudiants.FindAsync(int.Parse(etudiantId));
                if (etudiant == null)
                {
                    return NotFound(new { Message = "Étudiant non trouvé." });
                }

                // Mettre à jour les informations
                etudiant.Nom = updateDto.Nom ?? etudiant.Nom;
                etudiant.Prenom = updateDto.Prenom ?? etudiant.Prenom;
                etudiant.Email = updateDto.Email ?? etudiant.Email;
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    etudiant.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }

                _context.Entry(etudiant).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // DELETE: api/users/etudiants/{id}
            [HttpDelete("etudiants/{id}")]
            [Authorize(Policy = "AdminOnly")]
            public async Task<IActionResult> DeleteEtudiant(int id)
            {
                var etudiant = await _context.Etudiants
                    .Include(e => e.Identifiant)
                    .Include(e => e.Classe)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (etudiant == null)
                {
                    return NotFound(new { Message = "Étudiant non trouvé." });
                }

                // Supprimer l'étudiant et ses dépendances
                _context.Utilisateurs.Remove(etudiant);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // DELETE: api/users/enseignants/{id}
            [HttpDelete("enseignants/{id}")]
            [Authorize(Policy = "AdminOnly")]
            public async Task<IActionResult> DeleteEnseignant(int id)
            {
                var enseignant = await _context.Enseignants
                    .Include(e => e.Identifiant)
                    .Include(e => e.Cours)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (enseignant == null)
                {
                    return NotFound(new { Message = "Enseignant non trouvé." });
                }

                // Supprimer l'enseignant et ses dépendances
                _context.Utilisateurs.Remove(enseignant);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        // GET: api/users/etudiant/current (Accessible uniquement aux étudiants)
        [HttpGet("etudiant/current")]
        [Authorize(Policy = "EtudiantOnly")]
        public async Task<ActionResult<EtudiantResponseDto>> GetCurrentEtudiant()
        {
            // Récupérer l'ID de l'étudiant depuis le token
            var etudiantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(etudiantId))
            {
                return Unauthorized(new { Message = "ID de l'étudiant non trouvé dans le token." });
            }

            // Récupérer l'étudiant connecté avec ses informations
            var etudiant = await _context.Etudiants
                .Include(e => e.Identifiant) // Inclure l'identifiant
                .Include(e => e.Classe) // Inclure la classe
                .FirstOrDefaultAsync(e => e.Id == int.Parse(etudiantId));

            if (etudiant == null)
            {
                return NotFound(new { Message = "Étudiant non trouvé." });
            }

            // Mapper l'entité Etudiant vers EtudiantResponseDto
            var responseDto = new EtudiantResponseDto
            {
                Id = etudiant.Id,
                Nom = etudiant.Nom,
                Prenom = etudiant.Prenom,
                Email = etudiant.Email,
                CIN = etudiant.Identifiant.CIN,
                NomClasse = etudiant.Classe.Nom
            };

            return responseDto;
        }
        // GET: api/users/enseignant/current (Accessible uniquement aux enseignants)
        [HttpGet("enseignant/current")]
        [Authorize(Policy = "EnseignantOnly")]
        public async Task<ActionResult<EnseignantResponseDto>> GetCurrentEnseignant()
        {
            // Récupérer l'ID de l'enseignant depuis le token
            var enseignantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(enseignantId))
            {
                return Unauthorized(new { Message = "ID de l'enseignant non trouvé dans le token." });
            }

            // Récupérer l'enseignant connecté avec ses informations
            var enseignant = await _context.Enseignants
                .Include(e => e.Identifiant) // Inclure l'identifiant
                .Include(e => e.Cours) // Inclure les cours créés
                .FirstOrDefaultAsync(e => e.Id == int.Parse(enseignantId));

            if (enseignant == null)
            {
                return NotFound(new { Message = "Enseignant non trouvé." });
            }

            // Mapper l'entité Enseignant vers EnseignantResponseDto
            var responseDto = new EnseignantResponseDto
            {
                Id = enseignant.Id,
                Nom = enseignant.Nom,
                Prenom = enseignant.Prenom,
                Email = enseignant.Email,
                SuperKey = enseignant.Identifiant.SuperKey,
                CoursCrees = enseignant.Cours.Select(c => c.Titre).ToList() // Liste des titres des cours créés
            };

            return responseDto;
        }
    }
    public class CreateEnseignantDto
    {
        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string SuperKey { get; set; } // Identifiant de l'enseignant
    }
    public class CreateEtudiantDto
    {
        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string CIN { get; set; } // Identifiant de l'étudiant

        [Required]
        public string NomClasse { get; set; } // Nom de la classe (insensible à la casse)
    }
    public class UpdateUserDto
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Haché avant de sauvegarder
    }
    public class EnseignantResponseDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string SuperKey { get; set; }
        public List<string> CoursCrees { get; set; } // Noms des cours créés
    }
    public class EnseignantResponseDto2
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string SuperKey { get; set; } // Ajoutez uniquement les propriétés nécessaires
    }
    public class EtudiantResponseDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string CIN { get; set; }
        public string NomClasse { get; set; }
    }
    public class AdminDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string SuperKey { get; set; } // Si vous souhaitez inclure le CIN de l'identifiant
    }

}

