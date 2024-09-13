using IMS.Application.Abstractions;
using IMS.Domain.Customers;
using IMS.Domain.Users;

namespace IMS.Application.Users.Contracts;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
}
