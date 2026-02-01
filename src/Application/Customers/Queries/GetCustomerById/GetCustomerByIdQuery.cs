using Application.Common.Interfaces;
using Application.Customers.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(string Id) : IRequest<CustomerDetailDto?>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailDto?>
{
    private readonly INorthwindDbContext _context;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(INorthwindDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CustomerDetailDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == request.Id, cancellationToken);

        if (customer == null)
            return null;

        var dto = _mapper.Map<CustomerDetailDto>(customer);
        dto.TotalOrders = customer.Orders.Count;

        return dto;
    }
}
