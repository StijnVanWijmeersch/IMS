using IMS.Domain.Orders;
using IMS.Domain.Products;

namespace IMS.Domain.JoinTables;

public sealed class OrderProducts
{
    public ulong OrderId { get; set; }
    public ulong ProductId { get; set; }
    public int Quantity { get; set; }
    public Price Price { get; set; } = null!;

    // Navigation properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
