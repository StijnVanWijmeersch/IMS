using IMS.SharedKernel;

namespace IMS.Domain.Orders;

public sealed class Status : Entity, ISoftDelete
{
    public Name Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public IList<Order> Orders { get; private set; } = [];

    private Status() { }

    public static Status Create(string name)
    {
        var status = new Status
        {
            Name = new Name(name)
        };

        return status;
    }
}
