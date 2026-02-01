using Application.Common.Interfaces;
using Application.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDetailDto?>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
{
    private readonly INorthwindDbContext _context;

    public GetOrderByIdQueryHandler(INorthwindDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDetailDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.ShipViaNavigation)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == request.Id, cancellationToken);

        if (order == null)
            return null;

        var items = order.OrderDetails.Select(od => new OrderItemDto
        {
            ProductId = od.ProductId,
            ProductName = od.Product?.ProductName,
            UnitPrice = od.UnitPrice,
            Quantity = od.Quantity,
            Discount = od.Discount
        }).ToList();

        return new OrderDetailDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.CompanyName,
            EmployeeName = order.Employee != null ? $"{order.Employee.FirstName} {order.Employee.LastName}" : null,
            ShipperName = order.ShipViaNavigation?.CompanyName,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShipAddress = order.ShipAddress,
            ShipCity = order.ShipCity,
            ShipRegion = order.ShipRegion,
            ShipPostalCode = order.ShipPostalCode,
            ShipCountry = order.ShipCountry,
            Items = items,
            TotalAmount = items.Sum(i => i.ExtendedPrice)
        };
    }
}
