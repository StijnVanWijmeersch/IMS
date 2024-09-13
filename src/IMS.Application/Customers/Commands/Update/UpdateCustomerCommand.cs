using IMS.Application.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Commands.Update;

public sealed record UpdateCustomerCommand(ulong CustomerId, string FirstName, string LastName, string Email) : IRequest<Result<CustomerDto>>;
