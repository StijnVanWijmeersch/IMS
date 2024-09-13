using IMS.SharedKernel;

namespace IMS.Domain.Users;

public sealed record Password
{
    public string Value { get; init; }

    public Password(string value)
    {
        EnsureThat.IsNotNullOrEmpty(value);
        EnsureThat.IsValidPassword(value);
        Value = value;
    }
}
