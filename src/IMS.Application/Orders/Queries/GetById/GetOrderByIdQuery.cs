using IMS.Application.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Queries.GetById;

public sealed record GetOrderByIdQuery(ulong Id) : IRequest<Result<OrderDto>>;
