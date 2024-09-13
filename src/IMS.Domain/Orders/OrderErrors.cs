using IMS.SharedKernel;

namespace IMS.Domain.Orders;

public class OrderErrors
{
    public static Error StatusNotFound(ulong id) =>
        new Error("OrderErrors.StatusNotFound", $"Status with id {id} was not found");

    public static Error CreateOrderFailed(string message) =>
        new Error("OrderErrors.CreateOrderFailed", $"Failed to create order: {message}");

    public static Error OrderNotFound(ulong id) =>
        new Error("OrderErrors.OrderNotFound", $"Order with id {id} was not found");

    public static Error RemoveOrderFailed(ulong orderId, string message) =>
        new Error("OrderErrors.RemoveOrderFailed", $"Failed to remove order with id {orderId}. {message}");

    public static Error ProductOutOfStock(ulong id) =>
        new Error("OrderErrors.ProductOutOfStock", $"Product with id {id} is out of stock");
}
