using IMS.Application.Customers.Contracts;
using IMS.Domain.Customers;
using IMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Customers;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly IMSDbContext _context;
    private readonly DbSet<Customer> _customers;

    public CustomerRepository(IMSDbContext context)
    {
        _context = context;
        _customers = _context.Set<Customer>();
    }

    public void Add(Customer entity)
    {
        _customers.Add(entity);
    }

    public void AddRange(IEnumerable<Customer> entities)
    {
        _customers.AddRange(entities);
    }

    public async Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _customers.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> FindByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        return await _customers.AnyAsync(c => c.Email == email, cancellationToken);
    }

    public void Remove(Customer entity)
    {
        _customers.Remove(entity);
    }

    public void Update(Customer entity)
    {
        _customers.Update(entity);
    }
}
