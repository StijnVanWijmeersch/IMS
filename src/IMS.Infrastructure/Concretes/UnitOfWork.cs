using IMS.Application.Abstractions;
using IMS.Infrastructure.Contexts;

namespace IMS.Infrastructure.Concretes;

public sealed class UnitOfWork : IUnitOfWork
{

    private readonly IMSDbContext _context;

    public UnitOfWork(IMSDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
