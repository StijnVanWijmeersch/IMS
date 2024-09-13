using IMS.SharedKernel;

namespace IMS.Domain.Products;

public sealed record Description
{
    public string? Value { get; init; }

    public Description(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            value = null;
            return;
        }

        EnsureThat.IsValidDescription(value);

        Value = value;
    }
}
