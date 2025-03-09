using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Seuls les admins peuvent accéder à ces points de terminaison
    public class AdminsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Admins/me
        [HttpGet("me")]
        public async Task<ActionResult<Admin>> GetMyInfo()
        {
            var adminId = GetUserId();
            var admin = await _context.Admin.FindAsync(adminId);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // PUT: api/Admins/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyInfo(Admin admin)
        {
            var adminId = GetUserId();

            if (adminId != admin.Id)
            {
                return BadRequest();
            }

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(adminId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("id");
            return int.Parse(userIdClaim.Value);
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.Id == id);
        }
    }
}