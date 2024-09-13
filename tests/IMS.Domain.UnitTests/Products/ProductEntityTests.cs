using FluentAssertions;
using IMS.Domain.Categories;
using IMS.Domain.JoinTables;
using IMS.Domain.Products;
using IMS.Domain.Products.Events;

namespace StockManagement.Domain.UnitTests.Products;

public class ProductEntityTests
{
    // Arrange
    string name = "Product Name";
    string description = "Product Description";
    string shortDescription = "Product Short Description";
    string sku = "SKU123";
    bool inStock = true;
    int stockQuantity = 10;
    decimal price = 100.00m;
    string imageUrl = "https://www.example.com/image.jpg";


    [Fact]
    public void CreateProduct_WithValidData_ShouldCreateProduct()
    {
        // Act
        var product = Product.Create(name, description, shortDescription, sku, stockQuantity, price, imageUrl);

        // Assert
        product.Should().NotBeNull();
        product.Name.Value.Should().Be(name);
        product.Description.Value.Should().Be(description);
        product.ShortDescription.Value.Should().Be(shortDescription);
        product.Sku.Value.Should().Be(sku);
        product.InStock.Should().Be(inStock);
        product.StockQuantity.Should().Be(stockQuantity);
        product.Price.Value.Should().Be(price);
        product.Image.Value.Should().Be(imageUrl);
        product.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void CreateProduct_ShouldRaiseProductCreatedEvent()
    {
        // Act
        var product = Product.Create(name, description, shortDescription, sku, stockQuantity, price, imageUrl);

        var domainEvent = product.PopDomainEvents();

        // Assert
        domainEvent.Should().NotBeEmpty();
        domainEvent.Should().HaveCount(1);
        domainEvent.Single().Should().BeOfType<ProductCreatedEvent>();
    }

    [Fact]
    public void AddCategories_ShouldAddProductCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create("Category 1"),
            Category.Create("Category 2")
        };

        var product = Product.Create(name, description, shortDescription, sku, stockQuantity, price, imageUrl);

        var productCategories = new List<ProductCategory>
        {
            new ()
            {
               Category = categories[0],
               Product = product
            },
            new ()
            {
               Category = categories[1],
               Product = product
            },

        };

        // Act
        product.SetCategories(productCategories);

        // Assert
        product.ProductCategories.Should().NotBeEmpty();
        product.ProductCategories.Should().HaveSameCount(productCategories);
        product.ProductCategories.Should().AllBeOfType<ProductCategory>();
        product.ProductCategories.Should().OnlyHaveUniqueItems();
        product.ProductCategories.Should().Contain(productCategories);
        product.ProductCategories.Should().NotContainNulls();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void Create_Should_ThrowException_WhenNameIsEmtyOrNull(string value)
    {
        // Act
        Product CreateAction() => Product.Create(value, description, shortDescription, sku, stockQuantity, price, imageUrl);

        // Assert
        FluentActions.Invoking(CreateAction)
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void Create_Should_CreateNullDescription_WhenDescriptionIsNullOrEmpty(string value)
    {
        // Act
        var product = Product.Create(name, value, shortDescription, sku, stockQuantity, price, imageUrl);

        // Assert
        product.Description.Value.Should().BeNull();
    }

    [Fact]
    public void Create_Should_ThrowArgumentException_WhenDescriptionIsTooLong()
    {
        // Arrange
        string longDescription = new('A', 1001);

        // Act
        void CreateAction() => Product.Create(name, longDescription, shortDescription, sku, stockQuantity, price, imageUrl);

        // Assert
        FluentActions.Invoking(CreateAction)
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(longDescription);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void Create_Should_ThrowArgumentNullException_WhenSkuisNullOrEmpty(string value)
    {
        // Act
        Product CreateAction() => Product.Create(name, description, shortDescription, value, stockQuantity, price, imageUrl);

        // Assert
        FluentActions.Invoking(CreateAction)
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName(value);
    }


    [Fact]
    public void Create_Should_ThrowArgumentException_WhenPriceIsNegative()
    {
        // Arrange
        var value = -1.00m;
        // Act
        void CreateAction() => Product.Create(name, description, shortDescription, sku, stockQuantity, value, imageUrl);

        // Assert
        FluentActions.Invoking(CreateAction)
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(value));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void ImageURl_Should_HaveDefaultValue_WhenImageUrlIsNullOrEmpty(string? value)
    {
        // Arrange
        var defaultValue = "https://placehold.co/400.png?text=Image";
        // Act
        var product = Product.Create(name, description, shortDescription, sku, stockQuantity, price, value);

        // Assert
        product.Image.Value.Should().Match(defaultValue);
    }

    [Fact]
    public void Create_Should_ThrowArgumentException_WhenImageUrlIsNotValid()
    {
        // Arrange
        var invalidImageUrl = "invalid-url";
        // Act
        void CreateAction() => Product.Create(name, description, shortDescription, sku, stockQuantity, price, invalidImageUrl);

        // Assert
        FluentActions.Invoking(CreateAction)
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(invalidImageUrl);
    }

    [Fact]
    public void Update_Should_RaiseProductUpdatedEvent()
    {
        // Arrange
        var product = Product.Create(name, description, shortDescription, sku, stockQuantity, price, imageUrl);
        var createdDomainEvent = product.PopDomainEvents();

        // Act
        product.Update(name, description, shortDescription, stockQuantity, price, imageUrl);
        var updatedDomainEvent = product.PopDomainEvents();

        // Assert
        updatedDomainEvent.Should().NotBeEmpty();
        updatedDomainEvent.Should().HaveCount(1);
        updatedDomainEvent.Single().Should().BeOfType<ProductUpdatedEvent>();
    }

    [Fact]
    public void MarkAsRemoved_Should_RaiseProductRemovedEvent()
    {
        // Arrange
        var product = Product.Create(name, description, shortDescription, sku, stockQuantity, price, imageUrl);
        var createdDomainEvent = product.PopDomainEvents();

        // Act
        product.MarkAsRemoved();
        var removedDomainEvent = product.PopDomainEvents();

        // Assert
        removedDomainEvent.Should().NotBeEmpty();
        removedDomainEvent.Should().HaveCount(1);
        removedDomainEvent.Single().Should().BeOfType<ProductRemovedEvent>();

    }

}
