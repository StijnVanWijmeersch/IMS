using IMS.Domain.Customers;
using IMS.SharedKernel;

namespace IMS.Domain.Users;

public sealed class User : Entity, ISoftDelete
{
    public Name FirstName { get; private set; } = null!;
    public Name LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password HashedPassword { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; set; }

    private User() { }

    public static User Create(string firstName, string lastName, string email, string password)
    {
        var user = new User
        {
            FirstName = new Name(firstName),
            LastName = new Name(lastName),
            Email = new Email(email),
            HashedPassword = new Password(password)
        };

        return user;
    }

    public void MarkAsRemoved()
    {
        IsDeleted = true;
    }

    public void Update(string firstName, string lastName, string email, string password)
    {
        FirstName = new Name(firstName);
        LastName = new Name(lastName);
        Email = new Email(email);
        HashedPassword = new Password(password);
    }
}
