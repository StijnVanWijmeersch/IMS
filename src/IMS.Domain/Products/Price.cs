using IMS.SharedKernel;

namespace IMS.Domain.Products;

public sealed record Price
{
    public decimal Value { get; init; }

    public Price(decimal value)
    {
        EnsureThat.IsPositive((int)value);
        Value = value;
    }

    private Price() { }
}
