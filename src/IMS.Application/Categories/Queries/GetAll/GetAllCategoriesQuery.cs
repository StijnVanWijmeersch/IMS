using IMS.Application.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Queries.GetAll;

public sealed record GetAllCategoriesQuery(int Cursor, int PageSize) : IRequest<Result<Page<CategoryDto>>>;
