using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Cours>> CreateCours(Cours cours)
        {
            _context.Cours.Add(cours);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCours", new { id = cours.Id }, cours);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCours(int id, Cours cours)
        {
            if (id != cours.Id)
            {
                return BadRequest();
            }

            _context.Entry(cours).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCours(int id)
        {
            var cours = await _context.Cours.FindAsync(id);
            if (cours == null)
            {
                return NotFound();
            }

            _context.Cours.Remove(cours);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
