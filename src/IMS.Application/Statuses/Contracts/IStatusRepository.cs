using IMS.Application.Abstractions;
using IMS.Domain.Orders;
using IMS.SharedKernel;

namespace IMS.Application.Statuses.Contracts;

public interface IStatusRepository : IBaseRepository<Status>
{
    public Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken);
}
