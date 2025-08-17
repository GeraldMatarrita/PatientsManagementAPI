using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Data;

namespace GestionPacientesApi.Infrastructure.Repositories
{
    // Interface for Unit of Work pattern to manage repositories and database context
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        // Represents the database context and repositories for different entities
        private readonly ApplicationDbContext _context = context;
        // Repositories for different entities
        // IRepository for patients
        public IRepository<Patient> Patients { get; } = new Repository<Patient>(context);
        // IRepository for medical histories
        public IRepository<MedicalHistory> MedicalHistories { get; } = new Repository<MedicalHistory>(context);
        // IRepository for doctors
        public IRepository<Doctor> Doctors { get; } = new Repository<Doctor>(context);
        // IRepository for users
        public IRepository<User> Users { get; } = new Repository<User>(context);
        // Asynchronous method to save changes to the database
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        // Dispose method to release resources
        public void Dispose() => _context.Dispose();
    }
}