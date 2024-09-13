namespace IMS.API.Requests.Products;

public sealed record UpsertProductRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? ShortDescription { get; init; }
    public string Sku { get; init; } = null!;
    public bool InStock { get; init; }
    public int StockQuantity { get; init; }
    public decimal Price { get; init; }
    public string? Image { get; init; }
    public List<ulong> Categories { get; init; } = [];
    public List<ulong> Stores { get; init; } = [];
}
