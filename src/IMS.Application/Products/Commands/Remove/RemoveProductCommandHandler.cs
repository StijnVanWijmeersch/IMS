using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Products.Contracts;
using IMS.Domain.Products;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IMS.Application.Mappers;

namespace IMS.Application.Products.Commands.Remove;

internal sealed class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IIMSDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public RemoveProductCommandHandler(
        IIMSDbContext context,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _cacheService = cacheService;

    }

    public async Task<Result<ProductDto>> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
    {
        var id = request.ProductId;

        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existingProduct is null)
        {
            return Result.Failure<ProductDto>(ProductErrors.ProductNotFound(request.ProductId));
        }

        existingProduct.MarkAsRemoved();

        try
        {
            _productRepository.Remove(existingProduct);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"products", cancellationToken);

            return Result.Success(existingProduct.ToProductDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<ProductDto>(ProductErrors.RemoveProductFailed(request.ProductId, ex.Message));
        }
    }
}
