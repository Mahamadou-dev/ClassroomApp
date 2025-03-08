using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Enseignant> Enseignants { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Classe> Classes { get; set; }
        public DbSet<Cours> Cours { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Fichier> Fichiers { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Identifiant> Identifiants { get; set; }
        public DbSet<Lecon> Lecons { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Soumission> Soumissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Enseignant)
                .WithMany(e => e.Cours)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Forum>()
                .HasOne(f => f.Utilisateur)
                .WithMany(u => u.Forums)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Soumission>()
                .HasOne(s => s.Etudiant)
                .WithMany(e => e.Soumissions)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}