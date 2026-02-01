using Application.Common.Interfaces;
using Application.Products.DTOs;
using AutoMapper;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDetailDto?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDetailDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetWithDetailsAsync(request.Id, cancellationToken);

        if (product == null)
            return null;

        var dto = _mapper.Map<ProductDetailDto>(product);
        dto.CategoryName = product.Category?.CategoryName;
        dto.SupplierName = product.Supplier?.CompanyName;

        return dto;
    }
}
