using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Queries.GetAllFromStock;

public sealed record GetAllProductsFromStockQuery(int Cursor, int PageSize) : IRequest<Result<Page<ProductDto>>>;