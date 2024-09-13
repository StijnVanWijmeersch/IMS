using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Domain.Orders;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Orders.Queries.GetById;

internal sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetOrderByIdQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        OrderDto? cacheResult = await _cacheService
            .GetAsync<OrderDto>($"orders-{id}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }

        var order = await _context.Orders
        .AsNoTracking()
        .Where(order => order.Id == id)
        .Select(order => new OrderDto
        {
            Id = order.Id,
            PlacedAt = order.CreatedAt,

            Status = new Statuses.StatusDto
            {
                Name = order.Status.Name.Value,
            },

            Customer = new Customers.CustomerDto
            {
                FirstName = order.Customer.FirstName.Value,
                LastName = order.Customer.LastName.Value,
                Email = order.Customer.Email.Value,
            },

            OrderProducts = order.OrderProducts
                .Select(orderProduct => new OrderProductDto
                {
                    Product = new Products.ProductDto
                    {
                        Id = orderProduct.Product.Id,
                        Name = orderProduct.Product.Name.Value,
                        Sku = orderProduct.Product.Sku.Value,
                        InStock = orderProduct.Product.InStock,
                        Image = orderProduct.Product.Image.Value,
                        StockQuantity = orderProduct.Product.StockQuantity,
                        Price = orderProduct.Product.Price.Value,
                        Date = orderProduct.Product.CreatedAt,
                    },
                    Quantity = orderProduct.Quantity,
                }),
        })
        .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return Result.Failure<OrderDto>(OrderErrors.OrderNotFound(request.Id));
        }

        await _cacheService.SetAsync($"orders-{id}", order, cancellationToken);

        return Result.Success(order);
    }
}
