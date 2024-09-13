using IMS.Application.Categories.Contracts;
using IMS.Domain.Categories;
using IMS.Infrastructure.Contexts;
using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Categories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly IMSDbContext _context;
    private readonly DbSet<Category> _categories;

    public CategoryRepository(IMSDbContext context)
    {
        _context = context;
        _categories = _context.Set<Category>();
    }

    public void Add(Category entity)
    {
        _categories.Add(entity);
    }

    public void AddRange(IEnumerable<Category> entities)
    {
        _categories.AddRange(entities);
    }

    public Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _categories.AnyAsync(cat => cat.Name == name, cancellationToken);
    }

    public async Task<Category?> FindByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _categories.FirstOrDefaultAsync(cat => cat.Id == id, cancellationToken);
    }

    public async Task<bool> IsInUseAsync(ulong id, CancellationToken cancellationToken)
    {
        var category = await _categories
            .AsNoTracking()
            .Include(cat => cat.ProductCategories)
            .FirstAsync(cat => cat.Id == id, cancellationToken);

        return category.ProductCategories.Count > 0;
    }

    public void Remove(Category entity)
    {
        _categories.Remove(entity);
    }

    public void Update(Category entity)
    {
        _categories.Update(entity);
    }
}
