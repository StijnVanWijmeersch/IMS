using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Queries.GetTotalCount;

public sealed record GetTotalOrderCountQuery : IRequest<Result<int>>;