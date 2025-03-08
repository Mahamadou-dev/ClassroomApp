using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnseignantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnseignantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Enseignant>> CreateEnseignant(Enseignant enseignant)
        {
            _context.Enseignants.Add(enseignant);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetEnseignant", new { id = enseignant.Id }, enseignant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnseignant(int id, Enseignant enseignant)
        {
            if (id != enseignant.Id)
            {
                return BadRequest();
            }

            _context.Entry(enseignant).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnseignant(int id)
        {
            var enseignant = await _context.Enseignants.FindAsync(id);
            if (enseignant == null)
            {
                return NotFound();
            }

            _context.Enseignants.Remove(enseignant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
