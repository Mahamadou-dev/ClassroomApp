using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public enum RoleUtilisateur
    {
        Etudiant,
        Enseignant,
        Admin
    }

    public enum StatutEvaluation
    {
        NonRemise,
        Remise
    }

    public class Utilisateur
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public RoleUtilisateur Role { get; set; }

        // Champ de stockage pour RoleString
        private string? _roleString;

        // Propriété RoleString avec getter et setter
        public string RoleString
        {
            get => _roleString ?? Role.ToString();
            set => _roleString = value;
        }
        // Relations
        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public required Identifiant Identifiant { get; set; }
        public Fichier? PhotoProfilFichier { get; set; }
    }

    public class Etudiant : Utilisateur
    {
        // Relations
        public ICollection<Soumission> Soumissions { get; set; } = new List<Soumission>();
        public ICollection<Cours> Cours { get; set; } = new List<Cours>();

        [ForeignKey("Classe")]
        public int ClasseId { get; set; } // Ajout de la clé étrangère vers Classe
        public Classe Classe { get; set; } = null!; // Navigation vers Classe
    }

    public class Enseignant : Utilisateur
    {
        // Relations
        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
        public ICollection<Soumission> Soumissions { get; set; } = new List<Soumission>();
    }

    public class Admin : Utilisateur
    {
        // Relations
        public ICollection<Classe> Classes { get; set; } = new List<Classe>();
    }

    public class Classe
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Relations
        public ICollection<Etudiant> Etudiants { get; set; } = new List<Etudiant>();
        public ICollection<Cours> Cours { get; set; } = new List<Cours>();

        [ForeignKey("Admin")]
        public int AdminId { get; set; } // Ajout de la clé étrangère vers Admin
        public Admin Admin { get; set; } = null!; // Navigation vers Admin
    }

    public class Cours
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Relations
        [ForeignKey("Classe")]
        public int ClasseId { get; set; }
        public Classe Classe { get; set; } = null!;

        public Forum Forum { get; set; } = null!;
        public ICollection<Lecon> Lecons { get; set; } = new List<Lecon>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

        [ForeignKey("Enseignant")]
        public int EnseignantId { get; set; }
        public Enseignant Enseignant { get; set; } = null!;
    }

    public class Commentaire
    {
        public int Id { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        public DateTimeOffset DateCreation { get; set; } = DateTimeOffset.UtcNow;

        // Relations
        [ForeignKey("Lecon")]
        public int LeconId { get; set; }
        public Lecon Lecon { get; set; } = null!;

        [ForeignKey("Utilisateur")]
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }

    public class Evaluation
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTimeOffset DateLimite { get; set; }

        // Relations
        [ForeignKey("Cours")]
        public int CoursId { get; set; }
        public Cours Cours { get; set; } = null!;

        public ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();
        public ICollection<Soumission> Soumissions { get; set; } = new List<Soumission>();
    }

    public class Fichier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Chemin { get; set; } = string.Empty;

        [Required]
        public string TypeFichier { get; set; } = string.Empty;

        // Relations
        [ForeignKey("Lecon")]
        public int? LeconId { get; set; }
        public Lecon? Lecon { get; set; }

        [ForeignKey("Evaluation")]
        public int? EvaluationId { get; set; }
        public Evaluation? Evaluation { get; set; }

        [ForeignKey("Soumission")]
        public int? SoumissionId { get; set; } // Ajout de la clé étrangère vers Soumission
        public Soumission? Soumission { get; set; } // Navigation vers Soumission

        [ForeignKey("Utilisateur")]
        public int? UtilisateurId { get; set; }
        public Utilisateur? Utilisateur { get; set; }
    }

    public class Forum
    {
        public int Id { get; set; }

        // Relations
        [ForeignKey("Cours")]
        public int CoursId { get; set; }
        public Cours Cours { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

    public class Identifiant
    {
        public int Id { get; set; }

        public string? CIN { get; set; }
        public string? SuperKey { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        public bool EstUtilise { get; set; } = false; // Indique si l'identifiant a été utilisé

        [ForeignKey("Utilisateur")]
        public int? UtilisateurId { get; set; } // Clé étrangère vers Utilisateur
        public Utilisateur? Utilisateur { get; set; } // Navigation vers Utilisateur
    }

    public class Lecon
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Contenu { get; set; }

        // Relations
        [ForeignKey("Cours")]
        public int CoursId { get; set; }
        public Cours Cours { get; set; } = null!;

        public ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();
        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    }

    public class Note
    {
        public int Id { get; set; }

        public float Valeur { get; set; }

        // Relations
        [ForeignKey("Soumission")]
        public int SoumissionId { get; set; }
        public Soumission Soumission { get; set; } = null!;
    }

    public class Soumission
    {
        public int Id { get; set; }

        public DateTimeOffset DateSoumission { get; set; }

        // Relations
        [ForeignKey("Etudiant")]
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; } = null!;

        [ForeignKey("Evaluation")]
        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; } = null!;

        public ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();
        public Note? Note { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        public DateTimeOffset DatePublication { get; set; } = DateTimeOffset.UtcNow;

        // Relations
        [ForeignKey("Forum")]
        public int ForumId { get; set; }
        public Forum Forum { get; set; } = null!;

        [ForeignKey("Utilisateur")]
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}