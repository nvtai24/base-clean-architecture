using Application.Common.Interfaces;
using Application.Orders.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Orders.Commands.CreateOrder;

/// <summary>
/// Command to create a new order with multiple items
/// This demonstrates Unit of Work pattern with transaction management
/// </summary>
public record CreateOrderCommand(CreateOrderDto Dto) : IRequest<CreateOrderResultDto>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResultDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Validate customer exists
        var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId, cancellationToken);
        if (customer == null)
            throw new ApplicationException($"Customer with ID '{dto.CustomerId}' not found");

        // Validate all products exist and have sufficient stock
        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = new Dictionary<int, Product>();

        foreach (var productId in productIds)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new ApplicationException($"Product with ID {productId} not found");

            products[productId] = product;
        }

        // Check stock availability
        foreach (var item in dto.Items)
        {
            var product = products[item.ProductId];
            if (product.Discontinued)
                throw new ApplicationException($"Product '{product.ProductName}' is discontinued");

            if (product.UnitsInStock < item.Quantity)
                throw new ApplicationException(
                    $"Insufficient stock for '{product.ProductName}'. Available: {product.UnitsInStock}, Requested: {item.Quantity}");
        }

        // === BEGIN TRANSACTION ===
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Create Order
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                OrderDate = DateTime.UtcNow,
                RequiredDate = dto.RequiredDate,
                ShipVia = dto.ShipperId,
                Freight = dto.Freight ?? 0,
                ShipName = dto.ShipName ?? customer.CompanyName,
                ShipAddress = dto.ShipAddress ?? customer.Address,
                ShipCity = dto.ShipCity ?? customer.City,
                ShipRegion = dto.ShipRegion ?? customer.Region,
                ShipPostalCode = dto.ShipPostalCode ?? customer.PostalCode,
                ShipCountry = dto.ShipCountry ?? customer.Country
            };

            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Get OrderId

            // 2. Create OrderDetails and update stock
            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = products[item.ProductId];

                // Create order detail
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    UnitPrice = product.UnitPrice ?? 0,
                    Quantity = item.Quantity,
                    Discount = item.Discount
                };

                // EF Core needs direct DbSet access for OrderDetails
                // This is handled via the Order's navigation property
                order.OrderDetails.Add(orderDetail);

                // Calculate line total
                var lineTotal = orderDetail.UnitPrice * item.Quantity * (1 - (decimal)item.Discount);
                totalAmount += lineTotal;

                // 3. Reduce product stock
                product.UnitsInStock = (short?)((product.UnitsInStock ?? 0) - item.Quantity);
                _unitOfWork.Products.Update(product);
            }

            // Save all changes and commit transaction
            await _unitOfWork.CommitAsync(cancellationToken);
            // === END TRANSACTION ===

            return new CreateOrderResultDto
            {
                OrderId = order.OrderId,
                TotalAmount = totalAmount,
                ItemCount = dto.Items.Count,
                Message = $"Order #{order.OrderId} created successfully with {dto.Items.Count} items"
            };
        }
        catch (Exception)
        {
            // Rollback on any error
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
