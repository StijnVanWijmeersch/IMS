using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Domain.Categories;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Categories.Queries.GetAll;

internal sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<Page<CategoryDto>>>
{
    private const int NO_LIMIT = -1;

    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetAllCategoriesQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<Page<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        Page<CategoryDto>? cacheResult = await _cacheService
            .GetAsync<Page<CategoryDto>>($"categories-{request.Cursor}-{request.PageSize}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }

        bool hasNoLimit = request.PageSize == NO_LIMIT;
        IQueryable<Category> categories;

        if (hasNoLimit)
        {
            categories = _context.Categories
                .AsNoTracking();
        }

        else
        {
            categories = _context.Categories
            .AsNoTracking()
            .OrderBy(cat => cat.Id)
            .Where(cat => cat.Id >= (ulong)request.Cursor)
            .Take(request.PageSize + 1);
        }

        if (!categories.Any())
        {
            return Result.Success(new Page<CategoryDto>(request.Cursor, request.PageSize, Array.Empty<CategoryDto>())
            {
                IsFirstPage = true,
                IsLastPage = true
            });
        }

        var categoryDtos = await categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name.Value,

        }).ToListAsync(cancellationToken);

        ulong? cursor = categoryDtos[^1].Id;

        var values = hasNoLimit
            ? categoryDtos
            : categoryDtos.Take(request.PageSize);

        var page = new Page<CategoryDto>((int)cursor!, request.PageSize, values);

        await _cacheService.SetAsync($"categories-{request.Cursor}-{request.PageSize}", page, cancellationToken);

        return Result.Success(page);

    }
}
