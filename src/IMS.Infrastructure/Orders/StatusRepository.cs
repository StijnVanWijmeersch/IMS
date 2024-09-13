using IMS.Application.Statuses.Contracts;
using IMS.Domain.Orders;
using IMS.Infrastructure.Contexts;
using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Orders;

public sealed class StatusRepository : IStatusRepository
{
    private readonly IMSDbContext _context;
    private readonly DbSet<Status> _statuses;

    public StatusRepository(IMSDbContext context)
    {
        _context = context;
        _statuses = _context.Set<Status>();
    }

    public void Add(Status entity)
    {
        _statuses.Add(entity);
    }

    public void AddRange(IEnumerable<Status> entities)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _statuses.AnyAsync(
                       status => status.Id == id,
                       cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _statuses.AnyAsync(
                       status => status.Name == name,
                       cancellationToken);
    }

    public Task<Status?> FindByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Remove(Status entity)
    {
        _statuses.Remove(entity);
    }

    public void Update(Status entity)
    {
        _statuses.Update(entity);
    }
}
