using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Products.Contracts;
using IMS.Domain.Products;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IMS.Application.Mappers;
using IMS.Domain.JoinTables;

namespace IMS.Application.Products.Commands.Update;

internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IIMSDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateProductCommandHandler(
        IIMSDbContext context,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _context = context;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;

    }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.ExistsByIdAsync(request.Id, cancellationToken);

        if (!existingProduct)
        {
            return Result.Failure<ProductDto>(ProductErrors.ProductNotFound(request.Id));
        }

        var productToUpdate = await _context.Products
            .Include(product => product.ProductCategories)
            .FirstAsync(product => product.Id == request.Id, cancellationToken);

        var categoryIds = request.Categories;
        var storeIds = request.Stores;

        var categories = await _context.Categories
            .Where(category => categoryIds.Contains(category.Id))
            .ToListAsync(cancellationToken);

        productToUpdate.Update(
            request.Name, request.Description, request.ShortDescription,
            request.StockQuantity, request.Price,
            request.ImageUrl);

        var productCategories = categories.Select(category => new ProductCategory
        {
            ProductId = productToUpdate.Id,
            CategoryId = category.Id
        });

        productToUpdate.SetCategories(productCategories.ToList());

        try
        {
            _productRepository.Update(productToUpdate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"products", cancellationToken);

            return Result.Success(productToUpdate.ToProductDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<ProductDto>(ProductErrors.ProductUpdateFailed(ex.Message));
        }

    }
}
