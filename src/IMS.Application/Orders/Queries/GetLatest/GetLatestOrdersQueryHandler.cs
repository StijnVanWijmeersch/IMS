using IMS.Application.Abstractions;
using IMS.Application.Mappers;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Orders.Queries.GetLatest;

internal sealed class GetLatestOrdersQueryHandler : IRequestHandler<GetLatestOrdersQuery, Result<IEnumerable<OrderDto>>>
{
    private readonly IIMSDbContext _context;

    public GetLatestOrdersQueryHandler(IIMSDbContext context)
    {
        _context = context;
    }


    public async Task<Result<IEnumerable<OrderDto>>> Handle(GetLatestOrdersQuery request, CancellationToken cancellationToken)
    {
        var completed = new Name("Completed");

        var result = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Status)
            .Where(order => order.Status.Name == completed)
            .OrderByDescending(order => order.CreatedAt)
            .Take(request.Amount)
            .Select(order => new OrderDto
            {
                OrderProducts = order.OrderProducts.Select(orderProduct => new OrderProductDto
                {
                    Product = orderProduct.Product.ToProductDto()
                }),
            })
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<OrderDto>>(result);
    }
}
