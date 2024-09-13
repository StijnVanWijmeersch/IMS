using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers;
using IMS.Application.Customers.Contracts;
using IMS.Application.Mappers;
using IMS.Domain.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Commands.Update;

internal sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var id = request.CustomerId;

        var existingCustomer = await _customerRepository
            .FindByIdAsync(id, cancellationToken);

        if (existingCustomer is null)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.CustomerNotFound(request.CustomerId));
        }

        existingCustomer.Update(request.FirstName, request.LastName, request.Email);

        try
        {
            _customerRepository.Update(existingCustomer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"customers", cancellationToken);

            return Result.Success(existingCustomer.ToCustomerDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.UpdateCustomerFailed(request.CustomerId, ex.Message));
        }
    }
}
