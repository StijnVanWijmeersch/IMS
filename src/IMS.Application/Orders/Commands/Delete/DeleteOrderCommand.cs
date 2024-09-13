using IMS.Application.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Commands.Delete;

public sealed record DeleteOrderCommand(ulong OrderId) : IRequest<Result<OrderDto>>;
