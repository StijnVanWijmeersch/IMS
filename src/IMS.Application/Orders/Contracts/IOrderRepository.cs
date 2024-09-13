using IMS.Application.Abstractions;
using IMS.Domain.Orders;

namespace IMS.Application.Orders.Contracts;

public interface IOrderRepository : IBaseRepository<Order>
{
}
