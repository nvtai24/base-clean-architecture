using Application.Common.Interfaces;
using Application.Products.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand(CreateProductDto Dto) : IRequest<int>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            ProductName = request.Dto.ProductName,
            SupplierId = request.Dto.SupplierId,
            CategoryId = request.Dto.CategoryId,
            QuantityPerUnit = request.Dto.QuantityPerUnit,
            UnitPrice = request.Dto.UnitPrice,
            UnitsInStock = request.Dto.UnitsInStock,
            UnitsOnOrder = request.Dto.UnitsOnOrder,
            ReorderLevel = request.Dto.ReorderLevel,
            Discontinued = request.Dto.Discontinued
        };

        await _unitOfWork.Products.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.ProductId;
    }
}
