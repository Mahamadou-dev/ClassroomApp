using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Admins_AdminId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cours_Enseignants_EnseignantId",
                table: "Cours");

            migrationBuilder.DropForeignKey(
                name: "FK_Cours_Etudiants_EtudiantId",
                table: "Cours");

            migrationBuilder.DropForeignKey(
                name: "FK_Soumissions_Enseignants_EnseignantId",
                table: "Soumissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Soumissions_Etudiants_EtudiantId",
                table: "Soumissions");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Enseignants");

            migrationBuilder.DropTable(
                name: "Etudiants");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Utilisateurs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RoleString",
                table: "Utilisateurs",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Utilisateurs_AdminId",
                table: "Classes",
                column: "AdminId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cours_Utilisateurs_EnseignantId",
                table: "Cours",
                column: "EnseignantId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cours_Utilisateurs_EtudiantId",
                table: "Cours",
                column: "EtudiantId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Soumissions_Utilisateurs_EnseignantId",
                table: "Soumissions",
                column: "EnseignantId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Soumissions_Utilisateurs_EtudiantId",
                table: "Soumissions",
                column: "EtudiantId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Utilisateurs_AdminId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cours_Utilisateurs_EnseignantId",
                table: "Cours");

            migrationBuilder.DropForeignKey(
                name: "FK_Cours_Utilisateurs_EtudiantId",
                table: "Cours");

            migrationBuilder.DropForeignKey(
                name: "FK_Soumissions_Utilisateurs_EnseignantId",
                table: "Soumissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Soumissions_Utilisateurs_EtudiantId",
                table: "Soumissions");

            migrationBuilder.DropColumn(
                name: "RoleString",
                table: "Utilisateurs");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Utilisateurs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Utilisateurs_Id",
                        column: x => x.Id,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enseignants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enseignants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enseignants_Utilisateurs_Id",
                        column: x => x.Id,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Etudiants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etudiants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Etudiants_Utilisateurs_Id",
                        column: x => x.Id,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Admins_AdminId",
                table: "Classes",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cours_Enseignants_EnseignantId",
                table: "Cours",
                column: "EnseignantId",
                principalTable: "Enseignants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cours_Etudiants_EtudiantId",
                table: "Cours",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Soumissions_Enseignants_EnseignantId",
                table: "Soumissions",
                column: "EnseignantId",
                principalTable: "Enseignants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Soumissions_Etudiants_EtudiantId",
                table: "Soumissions",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
