using IMS.Application.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Commands.Create;

public sealed record CreateOrderCommand(
    ulong CustomerId,
    ulong StatusId,
    Dictionary<ulong, int> Products,
    List<ulong> Invoices
    ) : IRequest<Result<OrderDto>>;