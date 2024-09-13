using IMS.Application.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Queries.GetById;

public sealed record GetCategoryByIdQuery(ulong Id) : IRequest<Result<CategoryDto>>;
