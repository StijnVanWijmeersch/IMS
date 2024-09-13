using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories;
using IMS.Domain.Products;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Products.Queries.GetById;

internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetProductByIdQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        ProductDto? cacheResult = await _cacheService
           .GetAsync<ProductDto>($"products-{id}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }

        var product = await _context.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name.Value,
                Description = product.Description.Value,
                ShortDescription = product.ShortDescription.Value,
                Image = product.Image.Value,
                Sku = product.Sku.Value,
                InStock = product.InStock,
                StockQuantity = product.StockQuantity,
                Price = product.Price.Value,
                Date = product.CreatedAt,

                Categories = product.ProductCategories.Select(pc => new CategoryDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name.Value
                }),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>(ProductErrors.ProductNotFound(request.Id));
        }

        await _cacheService.SetAsync($"products-{id}", product, cancellationToken);

        return Result.Success(product);
    }
}
