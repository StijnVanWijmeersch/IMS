using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Queries.GetById;

public sealed record GetProductByIdQuery(ulong Id) : IRequest<Result<ProductDto>>;