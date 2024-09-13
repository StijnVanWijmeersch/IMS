using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories;
using IMS.Domain.Categories;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Categories.Queries.GetById;

internal sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetCategoryByIdQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        CategoryDto? cacheResult = await _cacheService
            .GetAsync<CategoryDto>($"categories-{request.Id}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }

        var id = request.Id;

        var category = await _context.Categories
            .AsNoTracking()
            .Where(cat => cat.Id == id)
            .Select(cat => new CategoryDto
            {
                Id = cat.Id,
                Name = cat.Name.Value
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.CategoryNotFound(request.Id));
        }

        await _cacheService.SetAsync($"categories-{request.Id}", category, cancellationToken);

        return Result.Success(category);
    }
}
