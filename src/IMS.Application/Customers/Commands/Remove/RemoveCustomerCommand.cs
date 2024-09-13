using IMS.Application.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Commands.Remove;

public sealed record RemoveCustomerCommand(ulong CustomerId) : IRequest<Result<CustomerDto>>;
