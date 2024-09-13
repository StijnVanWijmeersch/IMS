using IMS.Application.Users.Contracts;
using IMS.Domain.Customers;
using IMS.Domain.Users;
using IMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly IMSDbContext _context;
    private readonly DbSet<User> _users;

    public UserRepository(IMSDbContext context)
    {
        _context = context;
        _users = _context.Set<User>();
    }

    public void Add(User entity)
    {
        _users.Add(entity);
    }

    public void AddRange(IEnumerable<User> entities)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User?> FindByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        var exists = await _users.AnyAsync(c => c.Email == email, cancellationToken);
        return !exists;
    }

    public void Remove(User entity)
    {
        throw new NotImplementedException();
    }

    public void Update(User entity)
    {
        throw new NotImplementedException();
    }
}
