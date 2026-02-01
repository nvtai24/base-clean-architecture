using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly INorthwindDbContext _context;

    public DeleteProductCommandHandler(INorthwindDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == request.Id, cancellationToken);

        if (entity == null)
            return false;

        _context.Products.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
