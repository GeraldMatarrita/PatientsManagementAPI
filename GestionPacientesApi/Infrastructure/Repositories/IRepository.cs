using System.Linq.Expressions;

namespace GestionPacientesApi.Infrastructure.Repositories
{
    // Generic repository interface for CRUD operations
    public interface IRepository<T> where T : class
    {
        // Asynchronous methods for CRUD operations
        // Retrieves an entity by its ID
        Task<T> GetByIdAsync(int id);
        // Retrieves all entities of type T
        Task<IEnumerable<T>> GetAllAsync();
        // Finds entities based on a predicate
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        // Returns a queryable collection of entities
        IQueryable<T> Query();
        // Adds a new entity asynchronously
        Task AddAsync(T entity);
        // Updates an existing entity
        void Update(T entity);
        // Deletes an entity
        void Delete(T entity);
    }
}
