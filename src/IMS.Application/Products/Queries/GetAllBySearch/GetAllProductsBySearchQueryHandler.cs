using IMS.Application.Abstractions;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Products.Queries.GetAllBySearch;

internal sealed class GetAllProductsBySearchQueryHandler : IRequestHandler<GetAllProductsBySearchQuery, Result<IEnumerable<ProductDto>>>
{
    private readonly IIMSDbContext _context;

    public GetAllProductsBySearchQueryHandler(IIMSDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsBySearchQuery request, CancellationToken cancellationToken)
    {
        var queryName = new Name(request.Search);

        var products = await _context.Products
            .AsNoTracking()
            .Where(product => ((string)(object)product.Name).ToUpper().Contains(request.Search.ToUpper()))
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name.Value,
                Sku = product.Sku.Value,
                Price = product.Price.Value,
                StockQuantity = product.StockQuantity
            })
            .ToListAsync(cancellationToken);


        return Result.Success<IEnumerable<ProductDto>>(products);
    }
}
