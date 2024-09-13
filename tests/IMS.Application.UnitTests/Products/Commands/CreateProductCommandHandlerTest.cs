using FluentAssertions;
using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Products.Contracts;
using IMS.Domain.Products;
using Moq;
using IMS.Application.Products.Commands.Create;
using StockManagement.Application.Products.Commands.Create;

namespace StockManagement.Application.UnitTests.Products.Commands;

public class CreateProductCommandHandlerTest
{

    private string name = "Test Product";
    private string description = "Test Description";
    private string shortDescription = "Test Short Description";
    private string sku = "TestSku";
    private int stockQuantity = 10;
    private decimal price = 10.0m;
    private string imageUrl = "https://test.com/image.jpg";
    private List<ulong> categories = [ 1, 2 ];
    private List<ulong> stores = [ 1, 2 ];

    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IIMSDbContext> _contextMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    public CreateProductCommandHandlerTest()
    {
        _productRepositoryMock = new();
        _unitOfWorkMock = new();
        _contextMock = new();
        _cacheServiceMock = new();

    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_SkuExists()
    {
        // Arrange
        var command = new CreateProductCommand(name, description, shortDescription, sku, true, 10, price, imageUrl, categories, stores);
        var handler = new CreateProductCommandHandler(
                _productRepositoryMock.Object,
                _contextMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        _productRepositoryMock.Setup(x => x.ExistsBySkuAsync(It.IsAny<string>(), default))
            .ReturnsAsync(true);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.ProductAlreadyExists(sku));
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        
    }
}
