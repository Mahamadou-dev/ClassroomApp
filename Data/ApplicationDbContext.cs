using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Enseignant> Enseignants { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Classe> Classes { get; set; }
        public DbSet<Cours> Cours { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Fichier> Fichiers { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Identifiant> Identifiants { get; set; }
        public DbSet<Lecon> Lecons { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Soumission> Soumissions { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'héritage (stratégie TPH)
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");

            // Configuration du discriminator
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("RoleString")
                .HasValue<Etudiant>("Etudiant")
                .HasValue<Enseignant>("Enseignant")
                .HasValue<Admin>("Admin");

            // Configuration des relations Utilisateur - Identifiant
            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Identifiant)
                .WithOne(i => i.Utilisateur)
                .HasForeignKey<Identifiant>(i => i.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Etudiant - Classe
            modelBuilder.Entity<Etudiant>()
                .HasOne(e => e.Classe)
                .WithMany(c => c.Etudiants)
                .HasForeignKey(e => e.ClasseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des relations Classe - Admin
            modelBuilder.Entity<Classe>()
                .HasOne(c => c.Admin)
                .WithMany(a => a.Classes)
                .HasForeignKey(c => c.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des relations Cours - Enseignant
            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Enseignant)
                .WithMany(e => e.Cours)
                .HasForeignKey(c => c.EnseignantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des relations Forum - Cours
            modelBuilder.Entity<Forum>()
                .HasOne(f => f.Cours)
                .WithOne(c => c.Forum)
                .HasForeignKey<Forum>(f => f.CoursId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Leçon - Cours
            modelBuilder.Entity<Lecon>()
                .HasOne(l => l.Cours)
                .WithMany(c => c.Lecons)
                .HasForeignKey(l => l.CoursId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Commentaire - Leçon
            modelBuilder.Entity<Commentaire>()
                .HasOne(c => c.Lecon)
                .WithMany(l => l.Commentaires)
                .HasForeignKey(c => c.LeconId)
                .OnDelete(DeleteBehavior.Cascade);
               
            // Configuration des relations Soumission - Evaluation
            modelBuilder.Entity<Soumission>()
                .HasOne(s => s.Evaluation)
                .WithMany(e => e.Soumissions)
                .HasForeignKey(s => s.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Note - Soumission
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Soumission)
                .WithOne(s => s.Note)
                .HasForeignKey<Note>(n => n.SoumissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Fichier - Soumission
            modelBuilder.Entity<Fichier>()
                .HasOne(f => f.Soumission)
                .WithMany(s => s.Fichiers)
                .HasForeignKey(f => f.SoumissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des relations Fichier - Utilisateur (photo de profil)
            modelBuilder.Entity<Fichier>()
                .HasOne(f => f.Utilisateur)
                .WithOne(u => u.PhotoProfilFichier)
                .HasForeignKey<Fichier>(f => f.UtilisateurId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuration des index
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Identifiant>()
                .HasIndex(i => i.CIN)
                .IsUnique();

            modelBuilder.Entity<Identifiant>()
                .HasIndex(i => i.SuperKey)
                .IsUnique();

            modelBuilder.Entity<Classe>()
                .HasIndex(c => c.Nom)
                .IsUnique();

            modelBuilder.Entity<Cours>()
                .HasIndex(c => new { c.ClasseId, c.EnseignantId });

            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.CoursId);

            modelBuilder.Entity<Forum>()
                .HasIndex(f => f.CoursId);

            modelBuilder.Entity<Message>()
                .HasIndex(m => m.ForumId);

            modelBuilder.Entity<Message>()
                .HasIndex(m => m.UtilisateurId);

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