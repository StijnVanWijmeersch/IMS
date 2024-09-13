using IMS.Application.Categories;

namespace IMS.Application.Products;

public sealed record ProductDto()
{
    public ulong? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string Sku { get; set; } = null!;
    public bool InStock { get; set; }
    public int StockQuantity { get; set; }
    public decimal Price { get; set; }
    public DateTime? Date { get; set; }
    public string? Image { get; set; }

    //RELATIONSHIPS
    public IEnumerable<CategoryDto> Categories { get; set; } = [];
    public IEnumerable<ProductDto> RelatedProducts { get; set; } = [];

}
