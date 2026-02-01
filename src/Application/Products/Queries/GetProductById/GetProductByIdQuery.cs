using Application.Common.Interfaces;
using Application.Products.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDetailDto?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto?>
{
    private readonly INorthwindDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(INorthwindDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductDetailDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == request.Id, cancellationToken);

        if (product == null)
            return null;

        var dto = _mapper.Map<ProductDetailDto>(product);
        dto.CategoryName = product.Category?.CategoryName;
        dto.SupplierName = product.Supplier?.CompanyName;

        return dto;
    }
}
