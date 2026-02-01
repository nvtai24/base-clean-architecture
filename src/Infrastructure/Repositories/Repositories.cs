using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(NorthwindDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.SupplierId == supplierId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetDiscontinuedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Discontinued)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);
    }
}

/// <summary>
/// Category repository implementation
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(NorthwindDbContext context) : base(context) { }

    public async Task<Category?> GetWithProductsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);
    }
}

/// <summary>
/// Customer repository implementation
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(NorthwindDbContext context) : base(context) { }

    public async Task<Customer?> GetWithOrdersAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == id, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Country == country)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Order repository implementation
/// </summary>
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(NorthwindDbContext context) : base(context) { }

    public async Task<Order?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.ShipViaNavigation)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderDetails)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.OrderDate >= from && o.OrderDate <= to)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Employee repository implementation
/// </summary>
public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(NorthwindDbContext context) : base(context) { }

    public async Task<IEnumerable<Employee>> GetByTerritoryAsync(string territoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.Territories.Any(t => t.TerritoryId == territoryId))
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Supplier repository implementation
/// </summary>
public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(NorthwindDbContext context) : base(context) { }

    public async Task<IEnumerable<Supplier>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Country == country)
            .ToListAsync(cancellationToken);
    }
}
