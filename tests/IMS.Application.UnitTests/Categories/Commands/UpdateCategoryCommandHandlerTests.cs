using FluentAssertions;
using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories.Contracts;
using IMS.Domain.Categories;
using Moq;
using IMS.Application.Categories.Commands.Update;
using IMS.SharedKernel;

namespace StockManagement.Application.UnitTests.Categories.Commands;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    public UpdateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new();
        _unitOfWorkMock = new();
        _cacheServiceMock = new();
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_CategoryDoesNotExist()
    {
        var command = new UpdateCategoryCommand(1, "CategoryName");

        var handler = new UpdateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync((Category?)null);

        _categoryRepositoryMock.Setup(x => x.Update(It.IsAny<Category>()));
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default));

        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Never);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.CategoryNotFound(command.Id));
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResultWithCategoryDtoObject_When_CategoryIsUpdated()
    {
        var command = new UpdateCategoryCommand(1, "CategoryName");

        var handler = new UpdateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        var category = Category.Create("CategoryName");

        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(x => x.Update(It.IsAny<Category>()));
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default));

        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Once);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("CategoryName");
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_UpdateCallFails()
    {
        var command = new UpdateCategoryCommand(1, "CategoryName");

        var handler = new UpdateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);


        var category = Category.Create("CategoryName");
        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(x => x.Update(It.IsAny<Category>()))
            .Throws(new Exception("Update failed"));

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default));

        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.UpdateCategoryFailed(command.Id, "Update failed"));
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_SaveChangesCallFails()
    {
        var command = new UpdateCategoryCommand(1, "CategoryName");

        var handler = new UpdateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        var category = Category.Create("CategoryName");

        _categoryRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<ulong>(), default))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(x => x.Update(It.IsAny<Category>()));

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default))
            .Throws(new Exception("Save changes failed"));

        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.UpdateCategoryFailed(command.Id, "Save changes failed"));
    }
}
