using GestionPacientesApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionPacientesApi.Infrastructure.Repositories
{
    // Generic repository class implementing IRepository interface
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        // Represents the database context and the DbSet for the entity type T
        private readonly ApplicationDbContext _context = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        // Asynchronous methods for CRUD operations
        // Retrieves an entity by its ID
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        // Retrieves all entities of type T
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        // Finds entities based on a predicate
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();
        // Returns a queryable collection of entities
        public IQueryable<T> Query() => _dbSet.AsQueryable();
        // Adds a new entity asynchronously
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        // Updates an existing entity
        public void Update(T entity) => _dbSet.Update(entity);
        // Deletes an entity
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
