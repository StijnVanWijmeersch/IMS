using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Queries.GetIncomePerCategory;

public sealed record GetIncomePerCategoryQuery : IRequest<Result<Dictionary<string, decimal>>>;
