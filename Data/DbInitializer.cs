using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Backend.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Utilisateurs.AnyAsync())
            {
                return; // La base de données est déjà initialisée
            }

            // Créer un admin
            var admin = new Admin
            {
                Nom = "Admin",
                Prenom = "User",
                Email = "admin@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = RoleUtilisateur.Admin,
                RoleString = "Admin", // Initialiser RoleString explicitement
                Identifiant = new Identifiant
                {
                    SuperKey = "ADMIN123",
                    Type = "Admin"
                }
            };
            context.Utilisateurs.Add(admin);

            // Créer un enseignant
            var enseignant = new Enseignant
            {
                Nom = "Enseignant",
                Prenom = "User",
                Email = "enseignant@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Enseignant123!"),
                Role = RoleUtilisateur.Enseignant,
                RoleString = "Enseignant", // Initialiser RoleString explicitement
                Identifiant = new Identifiant
                {
                    CIN = "ENS123456",
                    Type = "Enseignant"
                }
            };
            context.Utilisateurs.Add(enseignant);

            // Créer un étudiant
            var etudiant = new Etudiant
            {
                Nom = "Etudiant",
                Prenom = "User",
                Email = "etudiant@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Etudiant123!"),
                Role = RoleUtilisateur.Etudiant,
                RoleString = "Etudiant", // Initialiser RoleString explicitement
                Identifiant = new Identifiant
                {
                    CIN = "ETU654321",
                    Type = "Etudiant"
                }
            };
            context.Utilisateurs.Add(etudiant);
            await context.SaveChangesAsync();
        }
    }
}