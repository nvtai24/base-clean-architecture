using Application.Common.Interfaces;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(int Id, UpdateProductDto Dto) : IRequest<bool>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            return false;

        entity.ProductName = request.Dto.ProductName;
        entity.SupplierId = request.Dto.SupplierId;
        entity.CategoryId = request.Dto.CategoryId;
        entity.QuantityPerUnit = request.Dto.QuantityPerUnit;
        entity.UnitPrice = request.Dto.UnitPrice;
        entity.UnitsInStock = request.Dto.UnitsInStock;
        entity.UnitsOnOrder = request.Dto.UnitsOnOrder;
        entity.ReorderLevel = request.Dto.ReorderLevel;
        entity.Discontinued = request.Dto.Discontinued;

        _unitOfWork.Products.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
