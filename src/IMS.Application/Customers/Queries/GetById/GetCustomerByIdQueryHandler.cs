using MediatR;
using Microsoft.EntityFrameworkCore;
using IMS.Application.Orders;
using IMS.Application.Abstractions;
using IMS.Application.Customers;
using IMS.Application.Caching;
using IMS.Domain.Customers;
using IMS.SharedKernel;

namespace IMS.Application.Customers.Queries.GetById;

internal sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetCustomerByIdQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        CustomerDto? cacheResult = await _cacheService
            .GetAsync<CustomerDto>($"customers-{id}", cancellationToken);

        if (cacheResult is not null)
        {
            return cacheResult;
        }

        var customer = await _context.Customers
        .AsNoTracking()
        .Where(customer => customer.Id == id)
        .Select(customer => new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName.Value,
            LastName = customer.LastName.Value,
            Email = customer.Email.Value,

            Orders = customer.Orders.Select(order => new OrderDto
            {
                Id = order.Id,

            })
        })
        .FirstOrDefaultAsync(cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.CustomerNotFound(request.Id));
        }

        await _cacheService.SetAsync($"customers-{id}", customer, cancellationToken);

        return Result.Success(customer);
    }
}
