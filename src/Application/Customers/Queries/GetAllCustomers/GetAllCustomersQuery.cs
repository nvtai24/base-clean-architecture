using Application.Common.Interfaces;
using Application.Customers.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery(
    string? Country = null,
    string? City = null,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetAllCustomersResult>;

public class GetAllCustomersResult
{
    public List<CustomerDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, GetAllCustomersResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetAllCustomersResult> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Customers.GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Country))
            query = query.Where(c => c.Country == request.Country);

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(c => c.City == request.City);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.CompanyName.Contains(request.Search) ||
                                     (c.ContactName != null && c.ContactName.Contains(request.Search)));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.CompanyName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new GetAllCustomersResult
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
