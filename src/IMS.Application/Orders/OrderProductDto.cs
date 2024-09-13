using IMS.Application.Products;

namespace IMS.Application.Orders;

public sealed record OrderProductDto
{
    public ProductDto? Product { get; init; }
    public int Quantity { get; init; }

}
