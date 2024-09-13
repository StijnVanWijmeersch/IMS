using IMS.Application.Customers;
using IMS.Application.Statuses;

namespace IMS.Application.Orders;

public sealed record OrderDto
{
    public ulong Id { get; set; }
    public CustomerDto Customer { get; set; } = null!;
    public StatusDto Status { get; set; } = null!;
    public DateTime PlacedAt { get; set; }
    public IEnumerable<OrderProductDto> OrderProducts { get; set; } = Enumerable.Empty<OrderProductDto>();

}
