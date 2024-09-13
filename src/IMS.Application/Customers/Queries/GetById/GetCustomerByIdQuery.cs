using IMS.Application.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Queries.GetById;

internal sealed record GetCustomerByIdQuery(ulong Id) : IRequest<Result<CustomerDto>>;
