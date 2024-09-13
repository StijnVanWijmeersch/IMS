using FluentAssertions;
using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories.Contracts;
using IMS.Domain.Categories;
using Moq;
using IMS.Application.Categories.Commands.Remove;

namespace StockManagement.Application.UnitTests.Categories.Commands;

public class RemoveCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    public RemoveCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new();
        _unitOfWorkMock = new();
        _cacheServiceMock = new();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenCategoryDoesNotExist()
    {

        var command = new RemoveCategoryCommand(1);
        var handler = new RemoveCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);


        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        _categoryRepositoryMock.Setup(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var result = await handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Never);
        _categoryRepositoryMock.Verify(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Never);

        _categoryRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.CategoryNotFound(command.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenCategoryIsInUse()
    {
        var command = new RemoveCategoryCommand(1);
        var handler = new RemoveCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object, _cacheServiceMock.Object);

        var category = Category.Create("CategoryName");
        category.Id = 1;

        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var result = await handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Never);

        _categoryRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.CategoryInUse(command.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenRemoveFails()
    {
        var command = new RemoveCategoryCommand(1);
        var handler = new RemoveCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        var category = Category.Create("CategoryName");
        category.Id = 1;

        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _categoryRepositoryMock.Setup(x => x.Remove(It.IsAny<Category>()))
            .Throws(new Exception("Remove failed"));

        var result = await handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.RemoveCategoryFailed(command.Id, "Remove failed"));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_When_SaveChangesFails()
    {
        var command = new RemoveCategoryCommand(1);
        var handler = new RemoveCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        var category = Category.Create("CategoryName");
        category.Id = 1;

        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception("SaveChanges failed"));

        var result = await handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.IsInUseAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()), Times.Once);


        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.RemoveCategoryFailed(command.Id, "SaveChanges failed"));
    }
}
