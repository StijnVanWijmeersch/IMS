using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers;
using IMS.Application.Customers.Contracts;
using IMS.Application.Mappers;
using IMS.Domain.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Commands.Remove;

internal sealed class RemoveCustomerCommandHandler : IRequestHandler<RemoveCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public RemoveCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;

    }

    public async Task<Result<CustomerDto>> Handle(RemoveCustomerCommand request, CancellationToken cancellationToken)
    {
        var id = request.CustomerId;

        var existingCustomer = await _customerRepository
            .FindByIdAsync(id, cancellationToken);

        if (existingCustomer is null)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.CustomerNotFound(request.CustomerId));
        }

        existingCustomer.MarkAsRemoved();

        try
        {
            _customerRepository.Remove(existingCustomer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"customers", cancellationToken);

            return Result.Success(existingCustomer.ToCustomerDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.RemoveCustomerFailed(request.CustomerId, ex.Message));
        }
    }
}
