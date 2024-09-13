using IMS.Application.Abstractions;
using IMS.Domain.Products;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IMS.Application.Categories;
using System.Linq.Expressions;

namespace IMS.Application.Products.Queries.GetAllByCategory;

public sealed class GetAllProductsByCategoryQueryHandler : IRequestHandler<GetAllProductsByCategoryQuery, Result<Page<ProductDto>>>
{

    private readonly IIMSDbContext _context;

    public GetAllProductsByCategoryQueryHandler(IIMSDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Page<ProductDto>>> Handle(GetAllProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Product, bool>> filter =
            product => product.ProductCategories
                .Any(pc => pc.CategoryId == request.CategoryId) &&
            product.Id >= (ulong)request.Cursor;

        var LastItemId = await _context.Products
            .AsNoTracking()
            .Where(product => product.ProductCategories.Any(pc => pc.CategoryId == request.CategoryId))
            .OrderBy(product => product.Id)
            .Select(p => p.Id)
            .LastOrDefaultAsync(cancellationToken);


        var products = _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Where(filter)
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

        return Result.Success(page);
    }
}
