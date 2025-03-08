using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Récupérer les infos de l'admin
    [HttpGet]
    public async Task<IActionResult> GetAdmin()
    {
        var admin = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Role == "Admin");
        if (admin == null)
        {
            return NotFound("Admin non trouvé");
        }
        return Ok(admin);
    }

    // Modifier les infos de l'admin
    [HttpPut]
    public async Task<IActionResult> UpdateAdmin([FromBody] Utilisateur updatedAdmin)
    {
        var admin = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Role == "Admin");
        if (admin == null)
        {
            return NotFound("Admin non trouvé");
        }

        admin.Nom = updatedAdmin.Nom;
        admin.Prenom = updatedAdmin.Prenom;
        admin.Email = updatedAdmin.Email;
        admin.Password = updatedAdmin.Password;

        _context.Utilisateurs.Update(admin);
        await _context.SaveChangesAsync();
        return Ok(admin);
    }
}
