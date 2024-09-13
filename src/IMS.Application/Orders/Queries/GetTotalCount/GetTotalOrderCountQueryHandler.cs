using IMS.Application.Abstractions;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Orders.Queries.GetTotalCount;

internal sealed class GetTotalOrderCountQueryHandler : IRequestHandler<GetTotalOrderCountQuery, Result<int>>
{
    private const int completedStatusId = 4;
    private readonly IIMSDbContext _context;

    public GetTotalOrderCountQueryHandler(IIMSDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(GetTotalOrderCountQuery request, CancellationToken cancellationToken)
    {
        var countResult = await _context.Orders
            .Where(order => order.StatusId == completedStatusId)
            .CountAsync(cancellationToken);

        return Result.Success(countResult);
    }
}
