using Application.Common.Interfaces;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
            return false;

        _unitOfWork.Products.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
