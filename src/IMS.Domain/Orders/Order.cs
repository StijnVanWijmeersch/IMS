using IMS.Domain.Customers;
using IMS.Domain.JoinTables;
using IMS.Domain.Products;
using IMS.SharedKernel;

namespace IMS.Domain.Orders;

public sealed class Order : Entity, ISoftDelete
{

    public ulong CustomerId { get; private set; }
    public ulong StatusId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Customer? Customer { get; private set; }
    public Status? Status { get; private set; }
    public IList<OrderProducts> OrderProducts { get; private set; } = [];

    private Order() { }

    public static Order Create(ulong customerId, ulong statusId)
    {
        var order = new Order
        {
            CustomerId = customerId,
            StatusId = statusId,
        };

        return order;
    }

    public void AddProducts(IList<ProductQuantity> products)
    {
        var orderProducts = products
            .Select(group => new OrderProducts
            {
                OrderId = Id,
                ProductId = group.Product.Id,
                Quantity = group.Quantity,
                Price = new Price(group.Product.Price.Value * group.Quantity)
            });

        OrderProducts = orderProducts.ToList();
    }

    public void MarkAsRemoved()
    {
        throw new NotImplementedException();
    }

    public void MarkAsCompleted()
    {
        StatusId = 4;
    }
}
