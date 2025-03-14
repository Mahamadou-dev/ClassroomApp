﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250311232431_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backend.Models.Classe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("Nom")
                        .IsUnique();

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("Backend.Models.Commentaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("DateCreation")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("LeconId")
                        .HasColumnType("int");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LeconId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Commentaires");
                });

            modelBuilder.Entity("Backend.Models.Cours", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClasseId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EnseignantId")
                        .HasColumnType("int");

                    b.Property<int?>("EtudiantId")
                        .HasColumnType("int");

                    b.Property<string>("Titre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("EnseignantId");

                    b.HasIndex("EtudiantId");

                    b.HasIndex("ClasseId", "EnseignantId");

                    b.ToTable("Cours");
                });

            modelBuilder.Entity("Backend.Models.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CoursId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("DateLimite")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CoursId");

                    b.ToTable("Evaluations");
                });

            modelBuilder.Entity("Backend.Models.Fichier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Chemin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EvaluationId")
                        .HasColumnType("int");

                    b.Property<int?>("LeconId")
                        .HasColumnType("int");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("SoumissionId")
                        .HasColumnType("int");

                    b.Property<string>("TypeFichier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UtilisateurId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationId");

                    b.HasIndex("LeconId");

                    b.HasIndex("SoumissionId");

                    b.HasIndex("UtilisateurId")
                        .IsUnique()
                        .HasFilter("[UtilisateurId] IS NOT NULL");

                    b.ToTable("Fichiers");
                });

            modelBuilder.Entity("Backend.Models.Forum", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CoursId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CoursId")
                        .IsUnique();

                    b.ToTable("Forums");
                });

            modelBuilder.Entity("Backend.Models.Identifiant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CIN")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("EstUtilise")
                        .HasColumnType("bit");

                    b.Property<string>("SuperKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UtilisateurId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CIN")
                        .IsUnique()
                        .HasFilter("[CIN] IS NOT NULL");

                    b.HasIndex("SuperKey")
                        .IsUnique()
                        .HasFilter("[SuperKey] IS NOT NULL");

                    b.HasIndex("UtilisateurId")
                        .IsUnique()
                        .HasFilter("[UtilisateurId] IS NOT NULL");

                    b.ToTable("Identifiants");
                });

            modelBuilder.Entity("Backend.Models.Lecon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CoursId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CoursId");

                    b.ToTable("Lecons");
                });

            modelBuilder.Entity("Backend.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("DatePublication")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("ForumId")
                        .HasColumnType("int");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ForumId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Backend.Models.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SoumissionId")
                        .HasColumnType("int");

                    b.Property<float>("Valeur")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("SoumissionId")
                        .IsUnique();

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Backend.Models.Soumission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateSoumission")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("EnseignantId")
                        .HasColumnType("int");

                    b.Property<int>("EtudiantId")
                        .HasColumnType("int");

                    b.Property<int>("EvaluationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EnseignantId");

                    b.HasIndex("EtudiantId");

                    b.HasIndex("EvaluationId");

                    b.ToTable("Soumissions");
                });

            modelBuilder.Entity("Backend.Models.Utilisateur", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("RoleString")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Utilisateurs", (string)null);

                    b.HasDiscriminator<string>("RoleString").HasValue("Utilisateur");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Backend.Models.Admin", b =>
                {
                    b.HasBaseType("Backend.Models.Utilisateur");

                    b.HasDiscriminator().HasValue("Admin");
                });

            modelBuilder.Entity("Backend.Models.Enseignant", b =>
                {
                    b.HasBaseType("Backend.Models.Utilisateur");

                    b.HasDiscriminator().HasValue("Enseignant");
                });

            modelBuilder.Entity("Backend.Models.Etudiant", b =>
                {
                    b.HasBaseType("Backend.Models.Utilisateur");

                    b.Property<int>("ClasseId")
                        .HasColumnType("int");

                    b.HasIndex("ClasseId");

                    b.HasDiscriminator().HasValue("Etudiant");
                });

            modelBuilder.Entity("Backend.Models.Classe", b =>
                {
                    b.HasOne("Backend.Models.Admin", "Admin")
                        .WithMany("Classes")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Backend.Models.Commentaire", b =>
                {
                    b.HasOne("Backend.Models.Lecon", "Lecon")
                        .WithMany("Commentaires")
                        .HasForeignKey("LeconId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithMany("Commentaires")
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lecon");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Backend.Models.Cours", b =>
                {
                    b.HasOne("Backend.Models.Classe", "Classe")
                        .WithMany("Cours")
                        .HasForeignKey("ClasseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Enseignant", "Enseignant")
                        .WithMany("Cours")
                        .HasForeignKey("EnseignantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Backend.Models.Etudiant", null)
                        .WithMany("Cours")
                        .HasForeignKey("EtudiantId");

                    b.Navigation("Classe");

                    b.Navigation("Enseignant");
                });

            modelBuilder.Entity("Backend.Models.Evaluation", b =>
                {
                    b.HasOne("Backend.Models.Cours", "Cours")
                        .WithMany("Evaluations")
                        .HasForeignKey("CoursId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cours");
                });

            modelBuilder.Entity("Backend.Models.Fichier", b =>
                {
                    b.HasOne("Backend.Models.Evaluation", "Evaluation")
                        .WithMany("Fichiers")
                        .HasForeignKey("EvaluationId");

                    b.HasOne("Backend.Models.Lecon", "Lecon")
                        .WithMany("Fichiers")
                        .HasForeignKey("LeconId");

                    b.HasOne("Backend.Models.Soumission", "Soumission")
                        .WithMany("Fichiers")
                        .HasForeignKey("SoumissionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithOne("PhotoProfilFichier")
                        .HasForeignKey("Backend.Models.Fichier", "UtilisateurId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Evaluation");

                    b.Navigation("Lecon");

                    b.Navigation("Soumission");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Backend.Models.Forum", b =>
                {
                    b.HasOne("Backend.Models.Cours", "Cours")
                        .WithOne("Forum")
                        .HasForeignKey("Backend.Models.Forum", "CoursId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cours");
                });

            modelBuilder.Entity("Backend.Models.Identifiant", b =>
                {
                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithOne("Identifiant")
                        .HasForeignKey("Backend.Models.Identifiant", "UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Backend.Models.Lecon", b =>
                {
                    b.HasOne("Backend.Models.Cours", "Cours")
                        .WithMany("Lecons")
                        .HasForeignKey("CoursId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cours");
                });

            modelBuilder.Entity("Backend.Models.Message", b =>
                {
                    b.HasOne("Backend.Models.Forum", "Forum")
                        .WithMany("Messages")
                        .HasForeignKey("ForumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithMany("Messages")
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Forum");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Backend.Models.Note", b =>
                {
                    b.HasOne("Backend.Models.Soumission", "Soumission")
                        .WithOne("Note")
                        .HasForeignKey("Backend.Models.Note", "SoumissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Soumission");
                });

            modelBuilder.Entity("Backend.Models.Soumission", b =>
                {
                    b.HasOne("Backend.Models.Enseignant", null)
                        .WithMany("Soumissions")
                        .HasForeignKey("EnseignantId");

                    b.HasOne("Backend.Models.Etudiant", "Etudiant")
                        .WithMany("Soumissions")
                        .HasForeignKey("EtudiantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Evaluation", "Evaluation")
                        .WithMany("Soumissions")
                        .HasForeignKey("EvaluationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Etudiant");

                    b.Navigation("Evaluation");
                });

            modelBuilder.Entity("Backend.Models.Etudiant", b =>
                {
                    b.HasOne("Backend.Models.Classe", "Classe")
                        .WithMany("Etudiants")
                        .HasForeignKey("ClasseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Classe");
                });

            modelBuilder.Entity("Backend.Models.Classe", b =>
                {
                    b.Navigation("Cours");

                    b.Navigation("Etudiants");
                });

            modelBuilder.Entity("Backend.Models.Cours", b =>
                {
                    b.Navigation("Evaluations");

                    b.Navigation("Forum")
                        .IsRequired();

                    b.Navigation("Lecons");
                });

            modelBuilder.Entity("Backend.Models.Evaluation", b =>
                {
                    b.Navigation("Fichiers");

                    b.Navigation("Soumissions");
                });

            modelBuilder.Entity("Backend.Models.Forum", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Backend.Models.Lecon", b =>
                {
                    b.Navigation("Commentaires");

                    b.Navigation("Fichiers");
                });

            modelBuilder.Entity("Backend.Models.Soumission", b =>
                {
                    b.Navigation("Fichiers");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("Backend.Models.Utilisateur", b =>
                {
                    b.Navigation("Commentaires");

                    b.Navigation("Identifiant")
                        .IsRequired();

                    b.Navigation("Messages");

                    b.Navigation("PhotoProfilFichier");
                });

            modelBuilder.Entity("Backend.Models.Admin", b =>
                {
                    b.Navigation("Classes");
                });

            modelBuilder.Entity("Backend.Models.Enseignant", b =>
                {
                    b.Navigation("Cours");

                    b.Navigation("Soumissions");
                });

            modelBuilder.Entity("Backend.Models.Etudiant", b =>
                {
                    b.Navigation("Cours");

                    b.Navigation("Soumissions");
                });
#pragma warning restore 612, 618
        }
    }
}
