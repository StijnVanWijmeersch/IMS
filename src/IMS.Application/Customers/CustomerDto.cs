using IMS.Application.Orders;

namespace IMS.Application.Customers;

public sealed record CustomerDto
{
    public ulong? Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;

    // Relationships
    public IEnumerable<OrderDto> Orders { get; set; } = Enumerable.Empty<OrderDto>();
}
