using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Mappers;
using IMS.Application.Orders;
using IMS.Application.Orders.Contracts;
using IMS.Domain.Orders;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Orders.Commands.Delete;

internal sealed class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<OrderDto>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var id = request.OrderId;

        var existingOrder = await _orderRepository
            .FindByIdAsync(id, cancellationToken);

        if (existingOrder is null)
        {
            return Result.Failure<OrderDto>(OrderErrors.OrderNotFound(request.OrderId));
        }

        existingOrder.MarkAsRemoved();

        try
        {
            _orderRepository.Remove(existingOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"orders", cancellationToken);

            return Result.Success(existingOrder.ToOrderDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<OrderDto>(OrderErrors.RemoveOrderFailed(request.OrderId, ex.Message));
        }
    }
}
