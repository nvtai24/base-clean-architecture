using Application.Common.Interfaces;
using Application.Products.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand(CreateProductDto Dto) : IRequest<int>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly INorthwindDbContext _context;

    public CreateProductCommandHandler(INorthwindDbContext context)
    {
        _context = context;
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

        _context.Products.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ProductId;
    }
}
