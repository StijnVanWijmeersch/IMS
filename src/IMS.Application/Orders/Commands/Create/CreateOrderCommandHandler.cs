using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers.Contracts;
using IMS.Application.Mappers;
using IMS.Application.Orders.Contracts;
using IMS.Application.Products.Contracts;
using IMS.Application.Statuses.Contracts;
using IMS.Domain.Customers;
using IMS.Domain.Orders;
using IMS.Domain.Products;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Orders.Commands.Create;

internal sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IStatusRepository _statusRespository;
    private readonly IIMSDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IStatusRepository statusRespository,
        IIMSDbContext context,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _statusRespository = statusRespository;
        _context = context;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customerExists = await _customerRepository
            .ExistsByIdAsync(request.CustomerId, cancellationToken);

        var statusExists = await _statusRespository
            .ExistsByIdAsync(request.StatusId, cancellationToken);

        if (!customerExists)
        {
            return Result.Failure<OrderDto>(CustomerErrors.CustomerNotFound(request.CustomerId));
        }

        if (!statusExists)
        {
            return Result.Failure<OrderDto>(OrderErrors.StatusNotFound(request.StatusId));
        }

        var relatedProducts = await _context.Products
            .Include(product => product.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .Where(product => request.Products.Keys.Contains(product.Id))
            .Select(product => new ProductQuantity
            {
                Product = product,
                Quantity = request.Products[product.Id]
            })
            .ToListAsync(cancellationToken);


        foreach (var productQuantity in relatedProducts)
        {
            var product = productQuantity.Product;

            var quantity = request.Products[product.Id];

            var result = product.DecreaseStockQuantity(quantity);

            if (result.IsFailure)
            {
                return Result.Failure<OrderDto>(OrderErrors.ProductOutOfStock(product.Id));
            }
        }

        try
        {
            var newOrder = Order.Create(request.CustomerId, request.StatusId);

            newOrder.AddProducts(relatedProducts);

            _orderRepository.Add(newOrder);

            _productRepository.UpdateRange(relatedProducts.Select(rp => rp.Product).ToList());

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"orders");

            return Result.Success(newOrder.ToOrderDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<OrderDto>(OrderErrors.CreateOrderFailed(ex.Message));
        }
    }
}
