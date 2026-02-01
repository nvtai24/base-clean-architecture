namespace Application.Common.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    IQueryable<T> GetQueryable();
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(object id, CancellationToken cancellationToken = default);
}
