using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Queries.GetAllSold;
public sealed record GetAllSoldProductsQuery() : IRequest<Result<int>>;
