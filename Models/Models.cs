using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{

    public class Utilisateur
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(255)]
        public string Prenom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)] // Adjust length as needed
        public string Password { get; set; } // Added Password property

        [Required]
        [MaxLength(50)] // Adjust length as needed
        public string Role { get; set; } // Added Role property

        // Ajout d'une propriété de navigation pour les forums
        public ICollection<Forum> Forums { get; set; } = new List<Forum>();
    }

    public class Etudiant : Utilisateur
    {
        public ICollection<Soumission> Soumissions { get; set; } = new List<Soumission>();
        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
    }

    public class Enseignant : Utilisateur
    {
        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
        public ICollection<Soumission> Soumissions { get; set; } = new List<Soumission>();
    }

    public class Admin : Utilisateur
    {
        public ICollection<Classe> Classes { get; set; } = new List<Classe>();
    }
    public class Classe
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; }

        public string? Description { get; set; }

        [ForeignKey("Admin")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }

        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
    }

    public class Cours
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; }

        public string? Description { get; set; }

        [ForeignKey("Classe")]
        public int ClasseId { get; set; }
        public Classe Classe { get; set; }

        [ForeignKey("Enseignant")]
        public int EnseignantId { get; set; }
        public Enseignant Enseignant { get; set; }

        public ICollection<Lecon> Lecons { get; set; } = new List<Lecon>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        public ICollection<Forum> Forums { get; set; } = new List<Forum>();
    }

    public class Evaluation
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset DateLimite { get; set; }

        [ForeignKey("Cours")]
        public int CoursId { get; set; }
        public Cours Cours { get; set; }

        public ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();
        public ICollection<Soumission> Soumissions { get; set; } = new List<Soumission>();
    }

    public class Fichier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; }

        [Required]
        public string Chemin { get; set; }

        [Required]
        public string TypeFichier { get; set; }

        [ForeignKey("Lecon")]
        public int? LeconId { get; set; }
        public Lecon Lecon { get; set; }

        [ForeignKey("Evaluation")]
        public int? EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }

        [ForeignKey("Soumission")]
        public int? SoumissionId { get; set; }
        public Soumission Soumission { get; set; }
    }

    public class Forum
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTimeOffset DatePublication { get; set; } = DateTimeOffset.UtcNow;

        [ForeignKey("Cours")]
        public int CoursId { get; set; }
        public Cours Cours { get; set; }

        [ForeignKey("Utilisateur")]
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }

    public class Identifiant
    {
        public int Id { get; set; }

        public string? CIN { get; set; }
        public string? SuperKey { get; set; }

        [Required]
        public string Type { get; set; }

        [ForeignKey("Utilisateur")]
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }

    public class Lecon
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titre { get; set; }

        public string? Description { get; set; }
        public string? Contenu { get; set; }

        [ForeignKey("Cours")]
        public int CoursId { get; set; }
        public Cours Cours { get; set; }

        public ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();
    }

    public class Note
    {
        public int Id { get; set; }
        public float Valeur { get; set; }

        [ForeignKey("Soumission")]
        public int SoumissionId { get; set; }
        public Soumission Soumission { get; set; }
    }

    public class Soumission
    {
        public int Id { get; set; }
        public DateTimeOffset DateSoumission { get; set; }

        public ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();

        [ForeignKey("Evaluation")]
        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }

        [ForeignKey("Etudiant")]
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
    }
}