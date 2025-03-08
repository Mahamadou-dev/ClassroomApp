using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentifiantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IdentifiantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Identifiant>>> GetIdentifiants()
        {
            return await _context.Identifiants.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Identifiant>> GetIdentifiant(int id)
        {
            var identifiant = await _context.Identifiants.FindAsync(id);

            if (identifiant == null)
            {
                return NotFound();
            }

            return identifiant;
        }

        [HttpPost]
        public async Task<ActionResult<Identifiant>> CreateIdentifiant(Identifiant identifiant)
        {
            _context.Identifiants.Add(identifiant);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetIdentifiant", new { id = identifiant.Id }, identifiant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIdentifiant(int id, Identifiant identifiant)
        {
            if (id != identifiant.Id)
            {
                return BadRequest();
            }

            _context.Entry(identifiant).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIdentifiant(int id)
        {
            var identifiant = await _context.Identifiants.FindAsync(id);
            if (identifiant == null)
            {
                return NotFound();
            }

            _context.Identifiants.Remove(identifiant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
