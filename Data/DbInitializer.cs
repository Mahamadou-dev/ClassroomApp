using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net; // Importer BCrypt.Net

public static class DbInitializer
{
    public static async Task Initialize(AppDbContext context)
    {
        // Vérifiez si la base de données existe, sinon créez-la
        await context.Database.EnsureCreatedAsync();

        // Si la base de données contient déjà des données, ne faites rien
        if (context.Utilisateurs.Any() || context.Identifiants.Any() || context.Classes.Any())
        {
            return;
        }

        // 1. Créez un admin
        var adminIdentifiant = new Identifiant { Type = "Admin", SuperKey = "ADMIN123" };
        var admin = new Admin
        {
            Nom = "Admin",
            Prenom = "User",
            Email = "admin@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin123"), // Hacher le mot de passe
            Role = RoleUtilisateur.Admin,
            Identifiant = adminIdentifiant // Associez l'identifiant à l'admin
        };
        context.Utilisateurs.Add(admin);
        await context.SaveChangesAsync();

        // 2. Créez des identifiants pour les enseignants et les étudiants
        var identifiants = new[]
        {
            new Identifiant { Type = "Enseignant", SuperKey = "TEACHER1", EstUtilise = false },
            new Identifiant { Type = "Enseignant", SuperKey = "TEACHER2", EstUtilise = false },
            new Identifiant { Type = "Etudiant", CIN = "STUDENT1", EstUtilise = false },
            new Identifiant { Type = "Etudiant", CIN = "STUDENT2", EstUtilise = false }
        };
        context.Identifiants.AddRange(identifiants);
        await context.SaveChangesAsync();

        // 3. Créez 2 classes pour l'admin
        var classes = new[]
        {
            new Classe { Nom = "Classe A", Description = "Première classe", AdminId = admin.Id },
            new Classe { Nom = "Classe B", Description = "Deuxième classe", AdminId = admin.Id }
        };
        context.Classes.AddRange(classes);
        await context.SaveChangesAsync();

        // 4. Créez 2 enseignants et associez leurs identifiants
        var enseignants = new[]
        {
            new Enseignant
            {
                Nom = "Enseignant",
                Prenom = "Un",
                Email = "enseignant1@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Teacher1"), // Hacher le mot de passe
                Role = RoleUtilisateur.Enseignant,
                Identifiant = identifiants[0] // Associez l'identifiant TEACHER1
            },
            new Enseignant
            {
                Nom = "Enseignant",
                Prenom = "Deux",
                Email = "enseignant2@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Teacher2"), // Hacher le mot de passe
                Role = RoleUtilisateur.Enseignant,
                Identifiant = identifiants[1] // Associez l'identifiant TEACHER2
            }
        };
        context.Utilisateurs.AddRange(enseignants);
        await context.SaveChangesAsync();

        // 5. Créez 2 étudiants et associez leurs identifiants
        var etudiants = new[]
        {
            new Etudiant
            {
                Nom = "Etudiant",
                Prenom = "Un",
                Email = "etudiant1@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Student1"), // Hacher le mot de passe
                Role = RoleUtilisateur.Etudiant,
                Identifiant = identifiants[2], // Associez l'identifiant STUDENT1
                ClasseId = classes[0].Id // Assigné à la première classe
            },
            new Etudiant
            {
                Nom = "Etudiant",
                Prenom = "Deux",
                Email = "etudiant2@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Student2"), // Hacher le mot de passe
                Role = RoleUtilisateur.Etudiant,
                Identifiant = identifiants[3], // Associez l'identifiant STUDENT2
                ClasseId = classes[1].Id // Assigné à la deuxième classe
            }
        };
        context.Utilisateurs.AddRange(etudiants);
        await context.SaveChangesAsync();

        // 6. Créez 2 cours, un pour chaque enseignant dans une classe différente
        var cours = new[]
        {
            new Cours
            {
                Titre = "Cours de Mathématiques",
                Description = "Introduction aux mathématiques",
                ClasseId = classes[0].Id, // Cours dans la première classe
                EnseignantId = enseignants[0].Id // Créé par le premier enseignant
            },
            new Cours
            {
                Titre = "Cours de Physique",
                Description = "Introduction à la physique",
                ClasseId = classes[1].Id, // Cours dans la deuxième classe
                EnseignantId = enseignants[1].Id // Créé par le deuxième enseignant
            }
        };
        context.Cours.AddRange(cours);
        await context.SaveChangesAsync();

        // 7. Créez des forums pour les cours
        var forums = new[]
        {
            new Forum { CoursId = cours[0].Id },
            new Forum { CoursId = cours[1].Id }
        };
        context.Forums.AddRange(forums);
        await context.SaveChangesAsync();

        Console.WriteLine("Base de données initialisée avec succès.");
    }
}