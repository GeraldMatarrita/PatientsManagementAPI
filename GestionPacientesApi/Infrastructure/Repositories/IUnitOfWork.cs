using GestionPacientesApi.Domain.Entities;

namespace GestionPacientesApi.Infrastructure.Repositories
{
    // Interface for Unit of Work pattern to manage repositories and database context
    public interface IUnitOfWork : IDisposable
    {
        // Repositories for different entities
        IRepository<Patient> Patients { get; }
        // Repository for medical histories
        IRepository<MedicalHistory> MedicalHistories { get; }
        // Repository for doctors
        IRepository<Doctor> Doctors { get; }
        // Repository for users
        IRepository<User> Users { get; }
        // Asynchronous method to save changes to the database
        Task<int> SaveChangesAsync();
    }
}
