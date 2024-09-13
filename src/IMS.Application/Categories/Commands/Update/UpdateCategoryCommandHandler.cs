using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories;
using IMS.Application.Categories.Contracts;
using IMS.Application.Mappers;
using IMS.Domain.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Commands.Update;

internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)

    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        var existingCategory = await _categoryRepository
            .FindByIdAsync(id, cancellationToken);

        if (existingCategory is null)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.CategoryNotFound(request.Id));
        }

        try
        {
            existingCategory.Update(request.Name);

            _categoryRepository.Update(existingCategory);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"categories", cancellationToken);

            return Result.Success(existingCategory.ToCategoryDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.UpdateCategoryFailed(request.Id, ex.Message));
        }

    }
}
