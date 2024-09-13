using IMS.SharedKernel;

namespace IMS.Domain.Products;

public class ProductErrors
{
    public static Error ProductAlreadyExists(string sku) =>
        new Error("ProductError.ProductAlreadyExists", $"Product with sku {sku} already exists.");

    public static Error ProductNotFound(ulong id) =>
        new Error("ProductError.ProductNotFound", $"Product with id {id} not found.");

    public static Error RemoveProductFailed(ulong productId, string message) =>
        new Error("ProductError.RemoveProductFailed", $"Failed to remove product with id {productId}. {message}");

    public static Error NotEnoughStockAvailable() =>
        new Error("ProductError.NotEnoughStockAvailable", "Not enough stock available.");

    public static Error ProductUpdateFailed(string message) =>
        new Error("ProductError.ProductUpdateFailed", $"Failed to update product. {message}");

    public static Error InvalidSku(string sku) =>
        new Error("ProductError.InvalidSku", $"Invalid SKU: {sku}");
}
