using IMS.SharedKernel;

namespace IMS.Domain.Products;

public sealed record ImageUrl
{
    public string Value { get; init; }

    public ImageUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = "https://placehold.co/400.png?text=Image";
            return;
        }

        EnsureThat.IsValidImage(value);

        Value = value;
    }
}
