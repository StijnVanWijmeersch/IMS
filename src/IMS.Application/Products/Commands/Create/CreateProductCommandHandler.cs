using MediatR;
using Microsoft.EntityFrameworkCore;
using IMS.Domain.JoinTables;
using System.Text.RegularExpressions;
using IMS.Application.Abstractions;
using IMS.Application.Mappers;
using IMS.Application.Products.Contracts;
using IMS.Application.Caching;
using IMS.Application.Products.Commands.Create;
using IMS.Application.Products;
using IMS.Domain.Products;
using IMS.SharedKernel;

namespace StockManagement.Application.Products.Commands.Create;

internal sealed partial class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRespository;
    private readonly IIMSDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    Regex skuRegex = SkuRegex();

    public CreateProductCommandHandler(
        IProductRepository productRespository,
        IIMSDbContext context,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _productRespository = productRespository;
        _context = context;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }


    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (skuRegex.IsMatch(request.Sku))
        {
            return Result.Failure<ProductDto>(ProductErrors.InvalidSku(request.Sku));
        }

        var existingProduct = await _productRespository.ExistsBySkuAsync(request.Sku, cancellationToken);

        if (existingProduct)
        {
            return Result.Failure<ProductDto>(ProductErrors.ProductAlreadyExists(request.Sku));
        }

        var categoryIds = request.Categories;
        var storeIds = request.Stores;
        
        var categories = await _context.Categories
            .Where(category => categoryIds.Contains(category.Id))
            .ToListAsync( cancellationToken );

        var newProduct = Product.Create(
            request.Name, request.Description, request.ShortDescription,
            request.Sku, request.StockQuantity, request.Price,
            request.ImageUrl);

        var productCategories = categories.Select(category => new ProductCategory
        {
            Product = newProduct,
            Category = category
        });

        newProduct.SetCategories( productCategories.ToList() );

        _productRespository.Add( newProduct );

        await _unitOfWork.SaveChangesAsync( cancellationToken );

        await _cacheService.RemoveByPrefixAsync("products", cancellationToken);

        return Result.Success( newProduct.ToProductDto() );
    }

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex SkuRegex();
}
