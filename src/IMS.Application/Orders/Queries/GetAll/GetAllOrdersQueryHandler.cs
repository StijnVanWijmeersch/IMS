using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers;
using IMS.Application.Products;
using IMS.Application.Statuses;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Orders.Queries.GetAll;

internal sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<Page<OrderDto>>>
{

    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetAllOrdersQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<Page<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        Page<OrderDto>? cacheResult = await _cacheService
            .GetAsync<Page<OrderDto>>($"orders-{request.Cursor}-{request.PageSize}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }


        var LastItemId = await _context.Orders
            .AsNoTracking()
            .OrderBy(order => order.Id)
            .Select(o => o.Id)
            .LastOrDefaultAsync(cancellationToken);


        var orders = _context.Orders
            .AsNoTracking()
            .OrderBy(order => order.Id)
            .Where(order => order.Id >= (ulong)request.Cursor)
            .Take(request.PageSize + 1);

        if (!orders.Any())
        {
            return Result.Success(new Page<OrderDto>(request.Cursor, request.PageSize, Array.Empty<OrderDto>())
            {
                IsFirstPage = true,
                IsLastPage = true
            });
        }

        var orderDtos = await orders.Select(o => new OrderDto
        {
            Id = o.Id,
            Customer = new CustomerDto
            {
                FirstName = o.Customer!.FirstName.Value,
                LastName = o.Customer.LastName.Value
            },
            Status = new StatusDto
            {
                Name = o.Status!.Name.Value
            },
            OrderProducts = o.OrderProducts.Select(op => new OrderProductDto
            {
                Product = new ProductDto
                {
                    Price = op.Product!.Price.Value
                },
                Quantity = op.Quantity
            }),

        }).ToListAsync(cancellationToken);

        ulong? cursor = orderDtos[^1].Id;

        var values = orderDtos.Take(request.PageSize);

        var page = new Page<OrderDto>((int)cursor!, request.PageSize, values)
        {
            IsFirstPage = request.Cursor <= 0,
            IsLastPage = cursor > LastItemId
        };

        await _cacheService.SetAsync($"orders-{request.Cursor}-{request.PageSize}", page, cancellationToken);

        return Result.Success(page);
    }
}
