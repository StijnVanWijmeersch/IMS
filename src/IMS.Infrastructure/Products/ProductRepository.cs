using IMS.Application.Products.Contracts;
using IMS.Domain.Products;
using IMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Products;

public sealed class ProductRepository : IProductRepository
{
    private readonly IMSDbContext _context;
    private readonly DbSet<Product> _products;

    public ProductRepository(IMSDbContext context)
    {
        _context = context;
        _products = _context.Set<Product>();
    }

    public void Add(Product entity)
    {
        _products.Add(entity);
    }

    public void AddRange(IEnumerable<Product> entities)
    {
        _products.AddRange(entities);
    }

    public async Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _products.AnyAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        Sku skuObj = new(sku);
        return await _products.AnyAsync(product => product.Sku == skuObj, cancellationToken);
    }

    public async Task<Product?> FindByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public void Remove(Product entity)
    {
        _products.Remove(entity);
    }

    public void RemoveRange(List<Product> relatedProducts)
    {
        _products.RemoveRange(relatedProducts);
    }

    public void Update(Product entity)
    {
        _products.Update(entity);
    }

    public void UpdateRange(List<Product> entities)
    {
        _products.UpdateRange(entities);
    }
}
