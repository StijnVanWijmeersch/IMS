using IMS.Domain.Orders;
using IMS.SharedKernel;

namespace IMS.Domain.Customers;

public sealed class Customer : Entity, ISoftDelete
{
    public Name FirstName { get; private set; } = null!;
    public Name LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public IList<Order> Orders { get; private set; } = [];

    private Customer() { }

    public static Customer Create(string firstName, string lastName, string email)
    {
        var customer = new Customer
        {
            FirstName = new Name(firstName),
            LastName = new Name(lastName),
            Email = new Email(email)
        };

        return customer;
    }

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = new Name(firstName);
        LastName = new Name(lastName);
        Email = new Email(email);
    }

    public void MarkAsRemoved()
    {
        // RAISE DOMAIN EVENT
    }
}
