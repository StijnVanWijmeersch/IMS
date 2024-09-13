using IMS.Application.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Commands.Create;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email) : IRequest<Result<CustomerDto>>;