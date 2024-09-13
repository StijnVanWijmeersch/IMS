using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories.Contracts;
using IMS.Application.Mappers;
using IMS.Domain.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Commands.Create;

internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = new Name(request.Name);

        var existingCategory = await _categoryRepository
            .ExistsByNameAsync(name, cancellationToken);

        if (existingCategory)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.CategoryAlreadyExists(request.Name));
        }

        try
        {
            var newCategory = Category.Create(request.Name);

            _categoryRepository.Add(newCategory);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"categories", cancellationToken);

            return Result.Success(newCategory.ToCategoryDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.CreateCategoryFailed(request.Name, ex.Message));
        }
    }
}
