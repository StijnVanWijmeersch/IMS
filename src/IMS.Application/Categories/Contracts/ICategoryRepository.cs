using IMS.Application.Abstractions;
using IMS.Domain.Categories;
using IMS.SharedKernel;

namespace IMS.Application.Categories.Contracts;

public interface ICategoryRepository : IBaseRepository<Category>
{
    public Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken);
    public Task<bool> IsInUseAsync(ulong id, CancellationToken cancellationToken);
}
