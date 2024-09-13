using FluentAssertions;
using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers.Contracts;
using IMS.Application.Orders.Contracts;
using IMS.Application.Products.Contracts;
using IMS.Application.Statuses.Contracts;
using IMS.Domain.Customers;
using IMS.Domain.Orders;
using Moq;
using IMS.Application.Orders.Commands.Create;

namespace StockManagement.Application.UnitTests.Orders.Commands;

public class CreateOrderCommandHandlerTest
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly Mock<IIMSDbContext> _contextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    public CreateOrderCommandHandlerTest()
    {
        _orderRepositoryMock = new();
        _customerRepositoryMock = new();
        _statusRepositoryMock = new();
        _contextMock = new();
        _unitOfWorkMock = new();
        _productRepositoryMock = new();
        _cacheServiceMock = new();
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_CustomerDoesNotExist()
    {
        // Arrange

        var prod = new Dictionary<ulong, int>()
        {
            {1, 1}
        };

        var command = new CreateOrderCommand(1, 1, prod, [1]);

        var handler = new CreateOrderCommandHandler(
                        _orderRepositoryMock.Object,
                        _productRepositoryMock.Object,
                        _customerRepositoryMock.Object,
                        _statusRepositoryMock.Object,
                        _contextMock.Object,
                        _unitOfWorkMock.Object,
                        _cacheServiceMock.Object)
        {

        };
        _customerRepositoryMock.Setup(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        _customerRepositoryMock.Verify(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default), Times.Once);
        _statusRepositoryMock.Verify(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default), Times.Once);
        _orderRepositoryMock.Verify(x => x.Add(It.IsAny<Order>()), Times.Never);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CustomerErrors.CustomerNotFound(command.CustomerId));
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_StatusDoesNotExist()
    {
        // Arrange
        var prod = new Dictionary<ulong, int>()
        {
            {1, 1}
        };

        var command = new CreateOrderCommand(1, 1, prod, [1]);

        var handler = new CreateOrderCommandHandler(
                        _orderRepositoryMock.Object,
                        _productRepositoryMock.Object,
                        _customerRepositoryMock.Object,
                        _statusRepositoryMock.Object,
                        _contextMock.Object,
                        _unitOfWorkMock.Object,
                        _cacheServiceMock.Object);

        _customerRepositoryMock.Setup(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync(true);

        _statusRepositoryMock.Setup(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        _customerRepositoryMock.Verify(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default), Times.Once);
        _statusRepositoryMock.Verify(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.StatusNotFound(command.StatusId));
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_AddCallFails()
    {
        // Arrange
        //var command = new CreateOrderCommand(1, 1, [1], [1]);

        //var handler = new CreateOrderCommandHandler(
        //                _orderRepositoryMock.Object,
        //                _customerRepositoryMock.Object,
        //                _statusRepositoryMock.Object,
        //                _contextMock.Object,
        //                _unitOfWorkMock.Object);

        //_customerRepositoryMock.Setup(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default))
        //    .ReturnsAsync(true);

        //_statusRepositoryMock.Setup(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default))
        //    .ReturnsAsync(true);

        //_orderRepositoryMock.Setup(x => x.Add(It.IsAny<Order>()))
        //    .Throws(new Exception("Add failed"));

        //// Act
        //var result = await handler.Handle(command, default);

        //// Assert
        //_unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        //_customerRepositoryMock.Verify(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default), Times.Once);
        //_statusRepositoryMock.Verify(x => x.ExistsByIdAsync(It.IsAny<ulong>(), default), Times.Once);
        //_orderRepositoryMock.Verify(x => x.Add(It.IsAny<Order>()), Times.Once);

        //result.IsFailure.Should().BeTrue();
        //result.Error.Should().Be(OrderErrors.CreateOrderFailed("Add failed"));


    }
}
