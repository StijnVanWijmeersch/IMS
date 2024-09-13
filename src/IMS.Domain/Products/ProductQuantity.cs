namespace IMS.Domain.Products;

public sealed record ProductQuantity
{
    public Product Product { get; init; } = null!;
    public int Quantity { get; init; }
}
