using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Queries.GetAllBySearch;

public sealed record GetAllProductsBySearchQuery(string Search, bool FromStock = false) : IRequest<Result<IEnumerable<ProductDto>>>;
