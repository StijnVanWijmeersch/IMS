using FluentAssertions;
using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories.Contracts;
using IMS.Domain.Categories;
using IMS.SharedKernel;
using Moq;
using IMS.Application.Categories.Commands.Create;

namespace StockManagement.Application.UnitTests.Categories.Commands;

public class CreateCategoryCommandHandlerTest
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    public CreateCategoryCommandHandlerTest()
    {
        _categoryRepositoryMock = new();
        _unitOfWorkMock = new();
        _cacheServiceMock = new();
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_CategoryAlreadyExists()
    {
        var command = new CreateCategoryCommand( "CategoryName" );

        var handler = new CreateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _cacheServiceMock.Object);

        _categoryRepositoryMock.Setup(x => x.ExistsByNameAsync(It.IsAny<Name>(), default))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default));
 
        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);

        _categoryRepositoryMock.Verify(x => x.Add(It.IsAny<Category>()), Times.Never);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.CategoryAlreadyExists(command.Name));
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResultWithCategoryDtoObject_WhenCategoryIsCreated()
    {
        var command = new CreateCategoryCommand( "CategoryName" );

        var handler = new CreateCategoryCommandHandler(
                       _categoryRepositoryMock.Object,
                       _unitOfWorkMock.Object, _cacheServiceMock.Object);

        _categoryRepositoryMock.Setup(x => x.ExistsByNameAsync(It.IsAny<Name>(), default))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default));
 
        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);

        _categoryRepositoryMock.Verify(x => x.Add(It.IsAny<Category>()), Times.Once);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_AddCallFails()
    {
        var command = new CreateCategoryCommand( "CategoryName" );

        var handler = new CreateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

        _categoryRepositoryMock.Setup(x => x.ExistsByNameAsync(It.IsAny<Name>(), default))
            .ReturnsAsync(false);

        _categoryRepositoryMock.Setup(x => x.Add(It.IsAny<Category>()))
            .Throws(new Exception("Add failed"));

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default));

        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        _categoryRepositoryMock.Verify(x => x.Add(It.IsAny<Category>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.CreateCategoryFailed(command.Name, "Add failed"));
    }

    [Fact]
    public async Task Handle_Should_Return_FailureResult_When_SaveChangesFails()
    {
        var command = new CreateCategoryCommand( "CategoryName" );

        var handler = new CreateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object, _cacheServiceMock.Object);

        _categoryRepositoryMock.Setup(x => x.ExistsByNameAsync(It.IsAny<Name>(), default))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default))
            .Throws(new Exception("SaveChanges failed"));

        var result = await handler.Handle(command, default);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Add(It.IsAny<Category>()), Times.Once);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.CreateCategoryFailed(command.Name, "SaveChanges failed"));
    }
}
