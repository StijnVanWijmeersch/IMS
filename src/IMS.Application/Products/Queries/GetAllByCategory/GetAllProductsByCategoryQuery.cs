using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Queries.GetAllByCategory;

public sealed record GetAllProductsByCategoryQuery(int Cursor, int PageSize, ulong CategoryId) : IRequest<Result<Page<ProductDto>>>;
