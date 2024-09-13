using IMS.Application.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Queries.GetLatest;

public sealed record GetLatestOrdersQuery(int Amount) : IRequest<Result<IEnumerable<OrderDto>>>;
