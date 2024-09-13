using IMS.Application.Abstractions;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Orders.Queries.GetIncomePerCategory;

internal sealed class GetIncomePerCategoryQueryHandler : IRequestHandler<GetIncomePerCategoryQuery, Result<Dictionary<string, decimal>>>
{
    private readonly IIMSDbContext _context;

    public GetIncomePerCategoryQueryHandler(IIMSDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Dictionary<string, decimal>>> Handle(GetIncomePerCategoryQuery request, CancellationToken cancellationToken)
    {

        return (Result<Dictionary<string, decimal>>)Result.Failure(Error.NullValue);

    }
}
