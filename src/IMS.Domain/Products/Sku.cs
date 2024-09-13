using IMS.SharedKernel;

namespace IMS.Domain.Products;

public sealed record Sku
{
    public string Value { get; init; }

    public Sku(string value)
    {
        EnsureThat.IsNotNullOrEmpty(value);
        Value = value;
    }
}
