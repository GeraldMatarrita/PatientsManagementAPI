using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPacientesApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdentificationNumber",
                table: "Patients",
                newName: "IdNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_IdentificationNumber",
                table: "Patients",
                newName: "IX_Patients_IdNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdNumber",
                table: "Patients",
                newName: "IdentificationNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_IdNumber",
                table: "Patients",
                newName: "IX_Patients_IdentificationNumber");
        }
    }
}
