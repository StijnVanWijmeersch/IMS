using IMS.Domain.JoinTables;
using IMS.Domain.Products.Events;
using IMS.SharedKernel;

namespace IMS.Domain.Products;
public sealed class Product : Entity, ISoftDelete
{
    public Name Name { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public Description ShortDescription { get; private set; } = null!;
    public Sku Sku { get; private set; } = null!;
    public bool InStock { get; private set; }
    public int StockQuantity { get; private set; }
    public Price Price { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ImageUrl Image { get; private set; } = null!;
    public bool IsDeleted { get; set; }

    // Navigation properties
    public IList<ProductCategory> ProductCategories { get; private set; } = [];
    public IList<OrderProducts> OrderProducts { get; private set; } = [];

    private Product() { }

    public static Product Create(
        string name, string? description, string? shortDescription,
        string sku, int stockQuantity, decimal price,
        string? imageUrl)
    {
        var product = new Product
        {
            Name = new Name(name),
            Description = new Description(description),
            ShortDescription = new Description(shortDescription),
            Sku = new Sku(sku),
            InStock = stockQuantity > 0,
            StockQuantity = stockQuantity,
            Price = new Price(price),
            Image = new ImageUrl(imageUrl)
        };

        product.RaiseDomainEvent(new ProductCreatedEvent(product));

        return product;
    }


    public void SetCategories(IList<ProductCategory> productCategories) => ProductCategories = productCategories;

    public void MarkAsRemoved()
    {
        RaiseDomainEvent(new ProductRemovedEvent(this));
    }

    public void Update(string name, string? description, string? shortDescription, int stockQuantity, decimal price, string? imageUrl)
    {

        Name = new Name(name);
        Description = new Description(description);
        ShortDescription = new Description(shortDescription);
        InStock = stockQuantity > 0;
        StockQuantity = stockQuantity;
        Price = new Price(price);
        Image = new ImageUrl(imageUrl);

        RaiseDomainEvent(new ProductUpdatedEvent(this));
    }

    public Result DecreaseStockQuantity(int amount)
    {
        if (StockQuantity - amount < 0)
        {
            return Result.Failure(ProductErrors.NotEnoughStockAvailable());
        }

        StockQuantity -= amount;

        if (StockQuantity == 0)
        {
            InStock = false;
        }

        RaiseDomainEvent(new ProductUpdatedEvent(this));

        return Result.Success();
    }
}
