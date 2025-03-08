using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity; // Si vous utilisez PasswordHasher
using Backend.Models;
using System.Security.Cryptography; // Si vous utilisez SHA256
using System.Text; // Si vous utilisez SHA256

public partial class InitializeAdminUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Choix de la méthode de hachage (PasswordHasher ou SHA256)
        var passwordHasher = new PasswordHasher<Utilisateur>();
        string password = "admin123"; // Remplacez par un mot de passe fort
        string hashedPassword = passwordHasher.HashPassword(null, password); // Utilisation de PasswordHasher

        // Ou :
        // using (SHA256 sha256Hash = SHA256.Create())
        // {
        //     byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        //     StringBuilder builder = new StringBuilder();
        //     for (int i = 0; i < bytes.Length; i++)
        //     {
        //         builder.Append(bytes[i].ToString("x2"));
        //     }
        //     hashedPassword = builder.ToString();
        // }

        migrationBuilder.InsertData(
            table: "Utilisateur",
            columns: new[] { "Nom", "Prenom", "Email", "Password", "Role" },
            values: new object[] { "Kalla", "Kanta", "admin@gmail.com", hashedPassword, "Admin" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM Utilisateur WHERE Email = 'admin@example.com'");
    }
}