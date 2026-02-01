using Application.Common.Interfaces;
using Application.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery(
    string? CustomerId = null,
    int? EmployeeId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetAllOrdersResult>;

public class GetAllOrdersResult
{
    public List<OrderDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, GetAllOrdersResult>
{
    private readonly INorthwindDbContext _context;

    public GetAllOrdersQueryHandler(INorthwindDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllOrdersResult> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CustomerId))
            query = query.Where(o => o.CustomerId == request.CustomerId);

        if (request.EmployeeId.HasValue)
            query = query.Where(o => o.EmployeeId == request.EmployeeId);

        if (request.FromDate.HasValue)
            query = query.Where(o => o.OrderDate >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(o => o.OrderDate <= request.ToDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer != null ? o.Customer.CompanyName : null,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
                Freight = o.Freight,
                ShipCity = o.ShipCity,
                ShipCountry = o.ShipCountry
            })
            .ToListAsync(cancellationToken);

        return new GetAllOrdersResult
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
