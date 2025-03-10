using System;
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
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Classe> Classes { get; set; }
        public DbSet<Cours> Cours { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Fichier> Fichiers { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Identifiant> Identifiants { get; set; }
        public DbSet<Lecon> Lecons { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Soumission> Soumissions { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'héritage (stratégie TPH)
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs"); // Une seule table pour tous les utilisateurs

            // Configuration du discriminator
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("RoleString")
                .HasValue<Etudiant>("Etudiant")
                .HasValue<Enseignant>("Enseignant")
                .HasValue<Admin>("Admin");

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

            modelBuilder.Entity<Soumission>()
                .HasOne(s => s.Evaluation)
                .WithMany(e => e.Soumissions)
                .HasForeignKey(s => s.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Identifiant>()
                .HasOne(i => i.Utilisateur)
                .WithOne(u => u.Identifiant)
                .HasForeignKey<Identifiant>(i => i.UtilisateurId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Cours>()
               .HasIndex(c => new { c.ClasseId, c.EnseignantId });

            modelBuilder.Entity<Classe>()
               .HasIndex(c => c.Nom)
               .IsUnique();

            // Configuration de la relation Fichier -> Utilisateur (pour la photo de profil)
            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.PhotoProfilFichier)
                .WithOne(f => f.Utilisateur)
                .HasForeignKey<Fichier>(f => f.UtilisateurId);

            modelBuilder.Entity<Fichier>()
                 .HasOne(f => f.Lecon)
                 .WithMany(l => l.Fichiers)
                 .HasForeignKey(f => f.LeconId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Fichier>()
                .HasOne(f => f.Evaluation)
                .WithMany(e => e.Fichiers)
                .HasForeignKey(f => f.EvaluationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fichier>()
                .HasOne(f => f.Soumission)
                .WithMany(s => s.Fichiers)
                .HasForeignKey(f => f.SoumissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des index
            modelBuilder.Entity<Cours>()
                .HasIndex(c => c.ClasseId);

            modelBuilder.Entity<Cours>()
                .HasIndex(c => c.EnseignantId);

            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.CoursId);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.Statut)
                .HasConversion<string>();

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
                .HasIndex(f => f.UtilisateurId);
        }
    }
}