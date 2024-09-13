using IMS.Application.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Queries.GetAll;

public sealed record GetAllOrdersQuery(int Cursor, int PageSize) : IRequest<Result<Page<OrderDto>>>;