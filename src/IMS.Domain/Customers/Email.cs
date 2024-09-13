using IMS.SharedKernel;

namespace IMS.Domain.Customers;

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        EnsureThat.IsNotNullOrEmpty(value);
        EnsureThat.IsValidEmail(value);
        Value = value;
    }
}
