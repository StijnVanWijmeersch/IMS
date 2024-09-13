using IMS.Application.Abstractions;
using IMS.Domain.Products;

namespace IMS.Application.Products.Contracts;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken);
    void UpdateRange(List<Product> relatedProducts);
    void RemoveRange(List<Product> relatedProducts);
}
