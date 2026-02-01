namespace Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface - manages transactions and repository access
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositories
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IEmployeeRepository Employees { get; }
    ISupplierRepository Suppliers { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
