using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/cours/by-classe/{nomClasse} (Accessible à tous les utilisateurs)
        [HttpGet("by-classe/{nomClasse}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CoursResponseDto>>> GetCoursByClasse(string nomClasse)
        {
            // Récupérer les cours de la classe spécifiée avec l'ID de l'enseignant
            var cours = await _context.Cours
                .Include(c => c.Classe)
                .Include(c => c.Enseignant)
                .Where(c => c.Classe.Nom.ToUpper() == nomClasse.ToUpper())
                .Select(c => new CoursResponseDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    NomClasse = c.Classe.Nom,
                    EnseignantId = c.EnseignantId
                })
                .ToListAsync();

            return cours;
        }

        // GET: api/cours (Accessible uniquement aux admins)
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<CoursResponseDto>>> GetAllCours()
        {
            // Récupérer tous les cours avec le nom de la classe
            var cours = await _context.Cours
                .Include(c => c.Classe)
                .Select(c => new CoursResponseDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    NomClasse = c.Classe.Nom,
                    EnseignantId = c.EnseignantId
                })
                .ToListAsync();

            return cours;
        }

        // GET: api/cours/etudiant (Accessible uniquement aux étudiants)
        [HttpGet("etudiant")]
        [Authorize(Policy = "EtudiantOnly")]
        public async Task<ActionResult<IEnumerable<CoursResponseDto>>> GetCoursForEtudiant()
        {
            // Récupérer l'ID de l'étudiant depuis le token
            var etudiantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(etudiantId))
            {
                return Unauthorized(new { Message = "ID de l'étudiant non trouvé dans le token." });
            }

            // Récupérer la classe de l'étudiant
            var etudiant = await _context.Etudiants
                .Include(e => e.Classe)
                .FirstOrDefaultAsync(e => e.Id == int.Parse(etudiantId));

            if (etudiant == null)
            {
                return NotFound(new { Message = "Étudiant non trouvé." });
            }

            // Récupérer les cours de la classe de l'étudiant
            var cours = await _context.Cours
                .Include(c => c.Classe)
                .Where(c => c.ClasseId == etudiant.ClasseId)
                .Select(c => new CoursResponseDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    NomClasse = c.Classe.Nom,
                    EnseignantId = c.EnseignantId
                })
                .ToListAsync();

            return cours;
        }

        // POST: api/cours (Accessible uniquement aux enseignants)
        // POST: api/cours (Accessible uniquement aux enseignants)
        [HttpPost]
        [Authorize(Policy = "EnseignantOnly")]
        public async Task<ActionResult<CoursResponseDto>> CreateCours(CreateCoursDto coursDto)
        {
            // Récupérer l'ID de l'enseignant depuis le token
            var enseignantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(enseignantId))
            {
                return Unauthorized(new { Message = "ID de l'enseignant non trouvé dans le token." });
            }

            // Vérifier que la classe existe
            var classe = await _context.Classes
                .FirstOrDefaultAsync(c => c.Nom.ToUpper() == coursDto.NomClasse.ToUpper());

            if (classe == null)
            {
                return BadRequest(new { Message = "Classe non trouvée." });
            }

            // Vérifier l'unicité du nom du cours dans la classe
            var coursExistant = await _context.Cours
                .FirstOrDefaultAsync(c => c.Titre.ToUpper() == coursDto.Titre.ToUpper() && c.ClasseId == classe.Id);

            if (coursExistant != null)
            {
                return Conflict(new { Message = "Un cours avec ce nom existe déjà dans cette classe." });
            }

            // Créer le cours
            var cours = new Cours
            {
                Titre = coursDto.Titre,
                Description = coursDto.Description,
                ClasseId = classe.Id,
                EnseignantId = int.Parse(enseignantId)
            };

            // Ajouter le cours à la base de données
            _context.Cours.Add(cours);
            await _context.SaveChangesAsync();

            // Créer un forum vide associé au cours
            var forum = new Forum
            {
                CoursId = cours.Id // Associer le forum au cours créé
            };

            // Ajouter le forum à la base de données
            _context.Forums.Add(forum);
            await _context.SaveChangesAsync();

            // Associer le forum au cours
            cours.Forum = forum;
            await _context.SaveChangesAsync();

            // Retourner le cours créé
            var responseDto = new CoursResponseDto
            {
                Id = cours.Id,
                Titre = cours.Titre,
                Description = cours.Description,
                NomClasse = classe.Nom,
                EnseignantId = cours.EnseignantId
            };

            return CreatedAtAction(nameof(GetCoursByClasse), new { nomClasse = classe.Nom }, responseDto);
        }

        // PUT: api/cours/{id} (Accessible uniquement aux enseignants)
        [HttpPut("{id}")]
        [Authorize(Policy = "EnseignantOnly")]
        public async Task<IActionResult> UpdateCours(int id, UpdateCoursDto coursDto)
        {
            // Récupérer l'ID de l'enseignant depuis le token
            var enseignantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(enseignantId))
            {
                return Unauthorized(new { Message = "ID de l'enseignant non trouvé dans le token." });
            }

            // Récupérer le cours existant
            var cours = await _context.Cours.FindAsync(id);
            if (cours == null)
            {
                return NotFound(new { Message = "Cours non trouvé." });
            }

            // Vérifier que l'enseignant est bien le propriétaire du cours
            if (cours.EnseignantId != int.Parse(enseignantId))
            {
                return Unauthorized(new { Message = "Vous n'êtes pas autorisé à modifier ce cours." });
            }

            // Vérifier l'unicité du nom du cours dans la classe
            if (!string.IsNullOrEmpty(coursDto.Titre))
            {
                var coursExistant = await _context.Cours
                    .FirstOrDefaultAsync(c => c.Titre.ToUpper() == coursDto.Titre.ToUpper() && c.ClasseId == cours.ClasseId && c.Id != id);

                if (coursExistant != null)
                {
                    return Conflict(new { Message = "Un cours avec ce nom existe déjà dans cette classe." });
                }

                cours.Titre = coursDto.Titre;
            }

            // Mettre à jour les autres propriétés
            cours.Description = coursDto.Description ?? cours.Description;

            _context.Entry(cours).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cours/{id} (Accessible uniquement aux enseignants)
        [HttpDelete("{id}")]
        [Authorize(Policy = "EnseignantOnly")]
        public async Task<IActionResult> DeleteCours(int id)
        {
            // Récupérer l'ID de l'enseignant depuis le token
            var enseignantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(enseignantId))
            {
                return Unauthorized(new { Message = "ID de l'enseignant non trouvé dans le token." });
            }

            // Récupérer le cours avec ses dépendances
            var cours = await _context.Cours
                .Include(c => c.Forum)
                .Include(c => c.Lecons)
                .Include(c => c.Evaluations)
                    .ThenInclude(e => e.Soumissions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cours == null)
            {
                return NotFound(new { Message = "Cours non trouvé." });
            }

            // Vérifier que l'enseignant est bien le propriétaire du cours
            if (cours.EnseignantId != int.Parse(enseignantId))
            {
                return Unauthorized(new { Message = "Vous n'êtes pas autorisé à supprimer ce cours." });
            }

            // Supprimer le cours et ses dépendances
            _context.Cours.Remove(cours);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/cours/enseignant (Accessible uniquement aux enseignants)
        [HttpGet("enseignant")]
        [Authorize(Policy = "EnseignantOnly")]
        public async Task<ActionResult<IEnumerable<CoursResponseDto>>> GetCoursForEnseignant()
        {
            // Récupérer l'ID de l'enseignant depuis le token
            var enseignantId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(enseignantId))
            {
                return Unauthorized(new { Message = "ID de l'enseignant non trouvé dans le token." });
            }

            // Récupérer tous les cours créés par l'enseignant connecté
            var cours = await _context.Cours
                .Include(c => c.Classe) // Inclure la classe associée
                .Where(c => c.EnseignantId == int.Parse(enseignantId)) // Filtrer par ID de l'enseignant
                .Select(c => new CoursResponseDto
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    NomClasse = c.Classe.Nom,
                    EnseignantId = c.EnseignantId
                })
                .ToListAsync();

            return cours;
        }
    }

    // DTOs
    public class CreateCoursDto
    {
        [Required]
        public string Titre { get; set; }

        public string Description { get; set; }

        [Required]
        public string NomClasse { get; set; } // Nom de la classe (insensible à la casse)
    }

    public class UpdateCoursDto
    {
        public string Titre { get; set; }
        public string Description { get; set; }
    }

    public class CoursResponseDto
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string NomClasse { get; set; }
        public int EnseignantId { get; set; }
    }
}