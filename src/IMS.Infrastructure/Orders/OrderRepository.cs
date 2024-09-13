using IMS.Application.Orders.Contracts;
using IMS.Domain.Orders;
using IMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Orders;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IMSDbContext _context;
    private readonly DbSet<Order> _orders;

    public OrderRepository(IMSDbContext context)
    {
        _context = context;
        _orders = _context.Set<Order>();
    }

    public void Add(Order entity)
    {
        _orders.Add(entity);
    }

    public void AddRange(IEnumerable<Order> entities)
    {
        _orders.AddRange(entities);
    }

    public async Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _orders.AnyAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<Order?> FindByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        return await _orders.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public void Remove(Order entity)
    {
        _orders.Remove(entity);
    }

    public void Update(Order entity)
    {
        _orders.Update(entity);
    }
}
