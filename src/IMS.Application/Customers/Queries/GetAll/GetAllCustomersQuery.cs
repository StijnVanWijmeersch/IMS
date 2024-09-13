using IMS.Application.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Queries.GetAll;

public sealed record GetAllCustomersQuery() : IRequest<Result<IEnumerable<CustomerDto>>>;
