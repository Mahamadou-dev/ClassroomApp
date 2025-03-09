using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
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
        public DbSet<Commentaire> Commentaires { get; set; } // Nouveau DbSet pour Commentaire

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'héritage
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");
            modelBuilder.Entity<Etudiant>().ToTable("Etudiants");
            modelBuilder.Entity<Enseignant>().ToTable("Enseignants");
            modelBuilder.Entity<Admin>().ToTable("Admins");

            // Configuration des relations
            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Enseignant)
                .WithMany(e => e.Cours)
                .HasForeignKey(c => c.EnseignantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Forum>()
                .HasOne(f => f.Utilisateur)
                .WithMany(u => u.Forums)
                .HasForeignKey(f => f.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Soumission>()
                .HasOne(s => s.Etudiant)
                .WithMany(e => e.Soumissions)
                .HasForeignKey(s => s.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Commentaire>()
                .HasOne(c => c.Lecon)
                .WithMany(l => l.Commentaires)
                .HasForeignKey(c => c.LeconId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Commentaire>()
                .HasOne(c => c.Utilisateur)
                .WithMany(u => u.Commentaires)
                .HasForeignKey(c => c.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation Utilisateur -> Identifiant
            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Identifiant)
                .WithOne()
                .HasForeignKey<Identifiant>(i => i.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict); // Utiliser Restrict au lieu de Cascade

            // Configuration de la relation Fichier -> Utilisateur (pour la photo de profil)
            modelBuilder.Entity<Fichier>()
                .HasOne(f => f.Utilisateur)
                .WithMany()
                .HasForeignKey(f => f.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des index
            modelBuilder.Entity<Cours>()
                .HasIndex(c => c.ClasseId);

            modelBuilder.Entity<Cours>()
                .HasIndex(c => c.EnseignantId);

            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.CoursId);

            modelBuilder.Entity<Forum>()
                .HasIndex(f => f.CoursId);

            modelBuilder.Entity<Forum>()
                .HasIndex(f => f.UtilisateurId);

            modelBuilder.Entity<Soumission>()
                .HasIndex(s => s.EvaluationId);

            modelBuilder.Entity<Soumission>()
                .HasIndex(s => s.EtudiantId);

            modelBuilder.Entity<Commentaire>()
                .HasIndex(c => c.LeconId);

            modelBuilder.Entity<Commentaire>()
                .HasIndex(c => c.UtilisateurId);

            modelBuilder.Entity<Fichier>()
                .HasIndex(f => f.LeconId);

            modelBuilder.Entity<Fichier>()
                .HasIndex(f => f.EvaluationId);

            modelBuilder.Entity<Fichier>()
                .HasIndex(f => f.SoumissionId);

            modelBuilder.Entity<Fichier>()
                .HasIndex(f => f.UtilisateurId); // Index pour la photo de profil
        }
    }
}