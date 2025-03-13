using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")] // Restreindre l'accès aux admins
    public class IdentifiantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IdentifiantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/identifiants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentifiantDto>>> GetIdentifiants()
        {
            var identifiants = await _context.Identifiants
                .Select(i => new IdentifiantDto
                {
                    Id = i.Id,
                    Type = i.Type,
                    SuperKey = i.SuperKey,
                    CIN = i.CIN,
                    EstUtilise = i.EstUtilise
                })
                .ToListAsync();

            return identifiants;
        }

        // GET: api/identifiants/with-possessors
        [HttpGet("with-possessors")]
        public async Task<ActionResult<IEnumerable<IdentifiantWithPossessorDto>>> GetIdentifiantsWithPossessors()
        {
            var identifiants = await _context.Identifiants
                .Include(i => i.Utilisateur)
                .Select(i => new IdentifiantWithPossessorDto
                {
                    Id = i.Id,
                    Type = i.Type,
                    SuperKey = i.SuperKey,
                    CIN = i.CIN,
                    EstUtilise = i.EstUtilise,
                    Possesseur = i.Utilisateur != null ? new UtilisateurDto
                    {
                        Id = i.Utilisateur.Id,
                        Nom = i.Utilisateur.Nom,
                        Prenom = i.Utilisateur.Prenom,
                        Email = i.Utilisateur.Email,
                        Role = i.Utilisateur.Role
                    } : null
                })
                .ToListAsync();

            return identifiants;
        }

        // GET: api/identifiants/by-role/{role}
        [HttpGet("by-role/{role}")]
        public async Task<ActionResult<IEnumerable<IdentifiantWithPossessorDto>>> GetIdentifiantsByRole(string role)
        {
            var identifiants = await _context.Identifiants
                .Include(i => i.Utilisateur)
                .Where(i => i.Utilisateur != null && i.Utilisateur.Role.ToString() == role)
                .Select(i => new IdentifiantWithPossessorDto
                {
                    Id = i.Id,
                    Type = i.Type,
                    SuperKey = i.SuperKey,
                    CIN = i.CIN,
                    EstUtilise = i.EstUtilise,
                    Possesseur = i.Utilisateur != null ? new UtilisateurDto
                    {
                        Id = i.Utilisateur.Id,
                        Nom = i.Utilisateur.Nom,
                        Prenom = i.Utilisateur.Prenom,
                        Email = i.Utilisateur.Email,
                        Role = i.Utilisateur.Role
                    } : null
                })
                .ToListAsync();

            return identifiants;
        }

        // POST: api/identifiants
        // POST: api/identifiants
        [HttpPost]
        public async Task<ActionResult<IdentifiantDto>> PostIdentifiant(IdentifiantDto identifiantDto)
        {
            // Validation du type
            if (string.IsNullOrEmpty(identifiantDto.Type))
            {
                return BadRequest(new { Message = "Le type est requis." });
            }

            // Validation des champs en fonction du type
            if (identifiantDto.Type == "Enseignant" && string.IsNullOrEmpty(identifiantDto.SuperKey))
            {
                return BadRequest(new { Message = "SuperKey est requis pour un enseignant." });
            }

            if (identifiantDto.Type == "Etudiant" && string.IsNullOrEmpty(identifiantDto.CIN))
            {
                return BadRequest(new { Message = "CIN est requis pour un étudiant." });
            }

            // Créer l'identifiant
            var identifiant = new Identifiant
            {
                Type = identifiantDto.Type,
                EstUtilise = identifiantDto.EstUtilise
            };

            // Définir SuperKey ou CIN en fonction du type
            if (identifiantDto.Type == "Enseignant")
            {
                identifiant.SuperKey = identifiantDto.SuperKey;
                identifiant.CIN = null; // Assurez-vous que CIN est null
            }
            else if (identifiantDto.Type == "Etudiant")
            {
                identifiant.CIN = identifiantDto.CIN;
                identifiant.SuperKey = null; // Assurez-vous que SuperKey est null
            }
            else
            {
                // Si le type n'est ni "Enseignant" ni "Etudiant", retourner une erreur
                return BadRequest(new { Message = "Le type doit être 'Enseignant' ou 'Etudiant'." });
            }

            // Ajouter l'identifiant à la base de données
            _context.Identifiants.Add(identifiant);
            await _context.SaveChangesAsync();

            // Retourner l'identifiant créé
            return CreatedAtAction(nameof(GetIdentifiants), new { id = identifiant.Id }, identifiantDto);
        }

        // DELETE: api/identifiants
        [HttpDelete]
        public async Task<IActionResult> DeleteIdentifiant(
            [FromQuery] string? cin = null,
            [FromQuery] string? superKey = null)
        {
            // Vérifier qu'un seul paramètre est fourni
            if (string.IsNullOrWhiteSpace(cin) && string.IsNullOrWhiteSpace(superKey))
            {
                return BadRequest(new { Message = "Vous devez fournir soit un CIN, soit une SuperKey." });
            }

            if (!string.IsNullOrWhiteSpace(cin) && !string.IsNullOrWhiteSpace(superKey))
            {
                return BadRequest(new { Message = "Vous ne pouvez pas fournir à la fois un CIN et une SuperKey." });
            }

            // Rechercher l'identifiant en fonction du paramètre fourni
            Identifiant? identifiant = null;
            if (!string.IsNullOrWhiteSpace(cin))
            {
                identifiant = await _context.Identifiants
                    .FirstOrDefaultAsync(i => i.CIN == cin);

            }
            else if (!string.IsNullOrWhiteSpace(superKey))
            {
                identifiant = await _context.Identifiants
                    .FirstOrDefaultAsync(i => i.SuperKey == superKey);
            }

            // Vérifier si l'identifiant a été trouvé
            if (identifiant == null)
            {
                return NotFound(new { Message = "Aucun identifiant correspondant trouvé." });
            }

            // Supprimer l'identifiant
            _context.Identifiants.Remove(identifiant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO pour un identifiant sans possesseur
    public class IdentifiantDto
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public string? SuperKey { get; set; }
        public string? CIN { get; set; }
        public bool EstUtilise { get; set; }
    }

    // DTO pour un identifiant avec son possesseur
    public class IdentifiantWithPossessorDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string SuperKey { get; set; }
        public string CIN { get; set; }
        public bool EstUtilise { get; set; }
        public UtilisateurDto? Possesseur { get; set; }
    }

    // DTO pour un utilisateur
    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public RoleUtilisateur Role { get; set; }
    }
}