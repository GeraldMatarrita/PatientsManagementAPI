using GestionPacientesApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionPacientesApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.IdNumber)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.MedicalHistories)
                .WithOne(h => h.Patient)
                .HasForeignKey(h => h.PatientId);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.MedicalHistories)
                .WithOne(h => h.Doctor)
                .HasForeignKey(h => h.DoctorId);
        }
    }
}
