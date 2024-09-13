namespace IMS.SharedKernel;

public sealed record Error(string Sender, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Null value is not allowed.");

    public static implicit operator Result(Error error) => Result.Failure(error);
}
