using IMS.Application.Abstractions;
using IMS.Domain.Customers;

namespace IMS.Application.Customers.Contracts;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
}
