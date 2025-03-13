using Backend.Data;
using Backend.Models;
using global::Backend.Data;
using global::Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
  

   
    
        [ApiController]
        [Route("api/[controller]")]
        public class ClassesController : ControllerBase
        {
            private readonly AppDbContext _context;

            public ClassesController(AppDbContext context)
            {
                _context = context;
            }

        // POST: api/classes
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ClasseResponseDto>> PostClasse(ClasseDto classeDto)
        {
            // Récupérer l'ID de l'admin à partir du token JWT
            var adminId = User.FindFirst("id")?.Value; // Utiliser "id" pour récupérer l'ID
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized(new { Message = "Admin ID non trouvé dans le token." });
            }
            // Vérifier si une classe de meme nom existe
            var existingClassroom = await _context.Classes.FirstOrDefaultAsync(c => c.Nom == classeDto.Nom);
            if (existingClassroom != null)
                return BadRequest(new { Message = "Une classe avec ce nom existe déjà " });

            // Mapper ClasseDto vers Classe
            var classe = new Classe
            {
                Nom = classeDto.Nom,
                Description = classeDto.Description,
                AdminId = int.Parse(adminId) // Associer l'admin à la classe
            };

            _context.Classes.Add(classe);
            await _context.SaveChangesAsync();

            // Mapper Classe vers ClasseResponseDto
            var responseDto = new ClasseResponseDto
            {
                Id = classe.Id,
                Nom = classe.Nom,
                Description = classe.Description,
                AdminId = classe.AdminId
            };

            return CreatedAtAction(nameof(GetClasseByName), new { nom = classe.Nom }, responseDto);
        }

        // PUT: api/classes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")] // Seul l'admin peut modifier une classe
        public async Task<IActionResult> PutClasse(int id, ClasseDto classeDto)
        {
            // Récupérer la classe existante
            var classe = await _context.Classes.FindAsync(id);
            if (classe == null)
            {
                return NotFound(new { Message = "Classe non trouvée." });
            }

            // Vérifier que l'admin est bien le propriétaire de la classe
            var adminId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(adminId) || classe.AdminId != int.Parse(adminId))
            {
                return Unauthorized(new { Message = "Vous n'êtes pas autorisé à modifier cette classe." });
            }

            // Mettre à jour les propriétés de la classe
            classe.Nom = classeDto.Nom;
            classe.Description = classeDto.Description;

            _context.Entry(classe).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/classes
        [HttpGet]
        [Authorize(Policy = "AdminAndEnseignant")] // Admin et enseignants peuvent voir toutes les classes
        public async Task<ActionResult<IEnumerable<ClasseResponseDto>>> GetClasses()
        {
            var classes = await _context.Classes.ToListAsync();

            // Mapper les classes vers ClasseResponseDto
            var responseDtos = classes.Select(c => new ClasseResponseDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Description = c.Description,
                AdminId = c.AdminId
            }).ToList();

            return responseDtos;
        }

        // GET: api/classes/by-name/{nom}
        [HttpGet("by-name/{nom}")]
        [Authorize]
        public async Task<ActionResult<ClasseResponseDto>> GetClasseByName(string nom)
        {
            // Rechercher la classe par nom (insensible à la casse)
            var classe = await _context.Classes
                .FirstOrDefaultAsync(c => c.Nom.ToUpper() == nom.ToUpper());

            if (classe == null)
            {
                return NotFound(new { Message = "Classe non trouvée." });
            }

            // Mapper Classe vers ClasseResponseDto
            var responseDto = new ClasseResponseDto
            {
                Id = classe.Id,
                Nom = classe.Nom,
                Description = classe.Description,
                AdminId = classe.AdminId
            };

            return responseDto;
        }

        // DELETE: api/classes/by-name/{nom}
        [HttpDelete("by-name/{nom}")]
            [Authorize(Policy = "AdminOnly")] // Seul l'admin peut supprimer une classe
            public async Task<IActionResult> DeleteClasseByName(string nom)
            {
                // Rechercher la classe par nom (insensible à la casse)
                var classe = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Nom.ToUpper() == nom.ToUpper());

                if (classe == null)
                {
                    return NotFound(new { Message = "Classe non trouvée." });
                }

                // Vérifier que l'admin est bien le propriétaire de la classe
                var adminId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(adminId) || classe.AdminId != int.Parse(adminId))
                {
                    return Unauthorized(new { Message = "Vous n'êtes pas autorisé à supprimer cette classe." });
                }

                _context.Classes.Remove(classe);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
        public class ClasseDto
        {
            public string Nom { get; set; }
            public string Description { get; set; }
        }
        public class ClasseResponseDto
        {
            public int Id { get; set; }
            public string Nom { get; set; }
            public string Description { get; set; }
            public int AdminId { get; set; } // ID de l'admin associé
        }
    }
