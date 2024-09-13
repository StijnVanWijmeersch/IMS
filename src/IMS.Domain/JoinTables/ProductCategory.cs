using IMS.Domain.Categories;
using IMS.Domain.Products;
using IMS.SharedKernel;

namespace IMS.Domain.JoinTables;

public sealed class ProductCategory
{
    public ulong ProductId { get; set; }
    public ulong CategoryId { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public Category? Category { get; set; }
}
