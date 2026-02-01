using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation - manages transactions and repository lifecycle
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly NorthwindDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    // Lazy-loaded repositories
    private IProductRepository? _products;
    private ICategoryRepository? _categories;
    private ICustomerRepository? _customers;
    private IOrderRepository? _orders;
    private IEmployeeRepository? _employees;
    private ISupplierRepository? _suppliers;

    public UnitOfWork(NorthwindDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IProductRepository Products =>
        _products ??= new ProductRepository(_context);

    public ICategoryRepository Categories =>
        _categories ??= new CategoryRepository(_context);

    public ICustomerRepository Customers =>
        _customers ??= new CustomerRepository(_context);

    public IOrderRepository Orders =>
        _orders ??= new OrderRepository(_context);

    public IEmployeeRepository Employees =>
        _employees ??= new EmployeeRepository(_context);

    public ISupplierRepository Suppliers =>
        _suppliers ??= new SupplierRepository(_context);

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begin a new database transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
