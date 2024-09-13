using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IMS.Application.Categories;

namespace IMS.Application.Products.Queries.GetAll;

internal sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<Page<ProductDto>>>
{
    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetAllProductsQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;

    }

    public async Task<Result<Page<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        Page<ProductDto>? cacheResult = await _cacheService
            .GetAsync<Page<ProductDto>>($"products-{request.Cursor}-{request.PageSize}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }

        var LastItemId = await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Select(p => p.Id)
            .LastOrDefaultAsync(cancellationToken);


        var products = _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Where(product => product.Id >= (ulong)request.Cursor)
            .Take(request.PageSize + 1);

        if (!products.Any())
        {
            return Result.Success(new Page<ProductDto>(request.Cursor, request.PageSize, Array.Empty<ProductDto>())
            {
                IsFirstPage = request.Cursor <= 0,
                IsLastPage = true
            });
        }

        var productDtos = await products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name.Value,
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

        }).ToListAsync(cancellationToken);

        ulong? cursor = productDtos[^1].Id;

        var values = productDtos.Take(request.PageSize);

        var page = new Page<ProductDto>((int)cursor!, request.PageSize, values)
        {
            IsFirstPage = request.Cursor <= 0,
            IsLastPage = cursor >= LastItemId
        };

        await _cacheService
                .SetAsync($"products-{request.Cursor}-{request.PageSize}", page, cancellationToken);

        return Result.Success(page);
    }
}