namespace IMS.SharedKernel;

public sealed record Name
{
    public string Value { get; init; }

    public Name(string value)
    {
        EnsureThat.IsNotNullOrEmpty(value);
        Value = value;
    }
}
