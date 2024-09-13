using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories.Contracts;
using IMS.Application.Mappers;
using IMS.Domain.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Commands.Remove;

internal sealed class RemoveCategoryCommandHandler : IRequestHandler<RemoveCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public RemoveCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<CategoryDto>> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        var existingCategory = await _categoryRepository
            .FindByIdAsync(id, cancellationToken);

        if (existingCategory is null)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.CategoryNotFound(request.Id));
        }

        var categoryIsInUse = await _categoryRepository
            .IsInUseAsync(id, cancellationToken);

        if (categoryIsInUse)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.CategoryInUse(request.Id));
        }

        try
        {
            _categoryRepository.Remove(existingCategory);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"categories", cancellationToken);

            return Result.Success(existingCategory.ToCategoryDto());
        }

        catch (Exception ex)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.RemoveCategoryFailed(request.Id, ex.Message));
        }
    }
}
