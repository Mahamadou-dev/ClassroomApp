using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClasseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClasseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Classe>> CreateClasse(Classe classe)
        {
            _context.Classes.Add(classe);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetClasse", new { id = classe.Id }, classe);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClasse(int id, Classe classe)
        {
            if (id != classe.Id)
            {
                return BadRequest();
            }

            _context.Entry(classe).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClasse(int id)
        {
            var classe = await _context.Classes.FindAsync(id);
            if (classe == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(classe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
