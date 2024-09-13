using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers;
using IMS.Application.Customers.Contracts;
using IMS.Application.Mappers;
using IMS.Domain.Customers;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Customers.Commands.Create;

internal sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var existingEmail = await _customerRepository.IsEmailUniqueAsync(email, cancellationToken);

        if (existingEmail)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.EmailIsNotUnique(request.Email));
        }

        try
        {
            var newCustomer = Customer.Create(request.FirstName, request.LastName, request.Email);

            _customerRepository.Add(newCustomer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync($"customers", cancellationToken);

            return Result.Success(newCustomer.ToCustomerDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.CreateCustomerFailed(request.FirstName, request.LastName, ex.Message));
        }

    }
}
