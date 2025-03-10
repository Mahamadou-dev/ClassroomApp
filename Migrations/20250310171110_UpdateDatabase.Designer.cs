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
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250310171110_UpdateDatabase")]
    partial class UpdateDatabase
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

                    b.HasIndex("ClasseId");

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

                    b.Property<string>("Statut")
                        .IsRequired()
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

                    b.Property<DateTimeOffset>("DatePublication")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CoursId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Forums");
                });

            modelBuilder.Entity("Backend.Models.Identifiant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CIN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SuperKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UtilisateurId")
                        .IsUnique();

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

                    b.HasIndex("SoumissionId");

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

                    b.Property<int>("Statut")
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

                    b.HasDiscriminator().HasValue("Etudiant");
                });

            modelBuilder.Entity("Backend.Models.Classe", b =>
                {
                    b.HasOne("Backend.Models.Admin", "Admin")
                        .WithMany("Classes")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Backend.Models.Commentaire", b =>
                {
                    b.HasOne("Backend.Models.Lecon", "Lecon")
                        .WithMany("Commentaires")
                        .HasForeignKey("LeconId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithMany("Commentaires")
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Restrict)
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
                        .HasForeignKey("EvaluationId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Backend.Models.Lecon", "Lecon")
                        .WithMany("Fichiers")
                        .HasForeignKey("LeconId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Backend.Models.Soumission", "Soumission")
                        .WithMany("Fichiers")
                        .HasForeignKey("SoumissionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithOne("PhotoProfilFichier")
                        .HasForeignKey("Backend.Models.Fichier", "UtilisateurId");

                    b.Navigation("Evaluation");

                    b.Navigation("Lecon");

                    b.Navigation("Soumission");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Backend.Models.Forum", b =>
                {
                    b.HasOne("Backend.Models.Cours", "Cours")
                        .WithMany("Forums")
                        .HasForeignKey("CoursId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithMany("Forums")
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cours");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Backend.Models.Identifiant", b =>
                {
                    b.HasOne("Backend.Models.Utilisateur", "Utilisateur")
                        .WithOne("Identifiant")
                        .HasForeignKey("Backend.Models.Identifiant", "UtilisateurId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

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

            modelBuilder.Entity("Backend.Models.Note", b =>
                {
                    b.HasOne("Backend.Models.Soumission", "Soumission")
                        .WithMany()
                        .HasForeignKey("SoumissionId")
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
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Backend.Models.Evaluation", "Evaluation")
                        .WithMany("Soumissions")
                        .HasForeignKey("EvaluationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Etudiant");

                    b.Navigation("Evaluation");
                });

            modelBuilder.Entity("Backend.Models.Classe", b =>
                {
                    b.Navigation("Cours");
                });

            modelBuilder.Entity("Backend.Models.Cours", b =>
                {
                    b.Navigation("Evaluations");

                    b.Navigation("Forums");

                    b.Navigation("Lecons");
                });

            modelBuilder.Entity("Backend.Models.Evaluation", b =>
                {
                    b.Navigation("Fichiers");

                    b.Navigation("Soumissions");
                });

            modelBuilder.Entity("Backend.Models.Lecon", b =>
                {
                    b.Navigation("Commentaires");

                    b.Navigation("Fichiers");
                });

            modelBuilder.Entity("Backend.Models.Soumission", b =>
                {
                    b.Navigation("Fichiers");
                });

            modelBuilder.Entity("Backend.Models.Utilisateur", b =>
                {
                    b.Navigation("Commentaires");

                    b.Navigation("Forums");

                    b.Navigation("Identifiant");

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
