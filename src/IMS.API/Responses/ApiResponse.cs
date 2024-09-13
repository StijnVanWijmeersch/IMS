namespace IMS.API.Responses;

public sealed record ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public int StatusCode { get; init; }
    public T? Result { get; init; }
    public string? ErrorMessage { get; init; }

}

public sealed record ApiResponse
{
    public bool Succeeded { get; init; }
    public int StatusCode { get; init; }
    public object? Result { get; init; }
    public string? ErrorMessage { get; init; }
}
