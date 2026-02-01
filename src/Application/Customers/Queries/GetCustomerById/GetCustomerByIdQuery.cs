using Application.Common.Interfaces;
using Application.Customers.DTOs;
using AutoMapper;
using MediatR;

namespace Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(string Id) : IRequest<CustomerDetailDto?>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDetailDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetWithOrdersAsync(request.Id, cancellationToken);

        if (customer == null)
            return null;

        var dto = _mapper.Map<CustomerDetailDto>(customer);
        dto.TotalOrders = customer.Orders.Count;

        return dto;
    }
}
