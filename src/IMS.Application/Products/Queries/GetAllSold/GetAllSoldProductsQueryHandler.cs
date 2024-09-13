using IMS.Application.Abstractions;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Products.Queries.GetAllSold;

internal sealed class GetAllSoldProductsQueryHandler : IRequestHandler<GetAllSoldProductsQuery, Result<int>>
{
    private const int COMPLETED_STATUS_ID = 4;

    private readonly IIMSDbContext _context;

    public GetAllSoldProductsQueryHandler(IIMSDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(GetAllSoldProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.Orders
            .Where(order => order.StatusId == COMPLETED_STATUS_ID)
            .SelectMany(order => order.OrderProducts)
            .CountAsync(cancellationToken);

        return Result.Success(result);
    }
}
