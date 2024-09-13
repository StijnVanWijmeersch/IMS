using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Queries.GetAll;

public sealed record GetAllProductsQuery(int Cursor, int PageSize) : IRequest<Result<Page<ProductDto>>>;
