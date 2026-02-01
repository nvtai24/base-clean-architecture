using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface INorthwindDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderDetail> OrderDetails { get; }
    DbSet<Product> Products { get; }
    DbSet<Region> Regions { get; }
    DbSet<Shipper> Shippers { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Territory> Territories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
