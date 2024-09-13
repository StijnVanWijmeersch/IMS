using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Customers;
using IMS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Customers.Queries.GetAll;

internal sealed class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<IEnumerable<CustomerDto>>>
{
    private readonly IIMSDbContext _context;
    private readonly ICacheService _cacheService;

    public GetAllCustomersQueryHandler(IIMSDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<CustomerDto>? cacheResult = await _cacheService
            .GetAsync<IEnumerable<CustomerDto>>("customers", cancellationToken);

        if (cacheResult is not null)
        {
            return Result.Success(cacheResult);
        }

        var customers = await _context.Customers
        .AsNoTracking()
        .Select(customer => new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName.Value,
            LastName = customer.LastName.Value,
            Email = customer.Email.Value
        })
        .ToListAsync(cancellationToken);

        await _cacheService.SetAsync("customers", customers, cancellationToken);

        return Result.Success<IEnumerable<CustomerDto>>(customers);
    }
}
