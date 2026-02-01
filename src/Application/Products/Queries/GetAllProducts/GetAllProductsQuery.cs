using Application.Common.Interfaces;
using Application.Products.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(
    int? CategoryId = null,
    int? SupplierId = null,
    bool? Discontinued = null,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetAllProductsResult>;

public class GetAllProductsResult
{
    public List<ProductDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsResult>
{
    private readonly INorthwindDbContext _context;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(INorthwindDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetAllProductsResult> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

        // Apply filters
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.SupplierId.HasValue)
            query = query.Where(p => p.SupplierId == request.SupplierId);

        if (request.Discontinued.HasValue)
            query = query.Where(p => p.Discontinued == request.Discontinued);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.ProductName.Contains(request.Search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.ProductName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new GetAllProductsResult
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
