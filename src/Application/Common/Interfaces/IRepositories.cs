using Domain.Entities;

namespace Application.Common.Interfaces;

/// <summary>
/// Product repository with specific query methods
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetDiscontinuedAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Category repository
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetWithProductsAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Customer repository
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetWithOrdersAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
}

/// <summary>
/// Order repository
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}

/// <summary>
/// Employee repository
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetByTerritoryAsync(string territoryId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Supplier repository
/// </summary>
public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
}
