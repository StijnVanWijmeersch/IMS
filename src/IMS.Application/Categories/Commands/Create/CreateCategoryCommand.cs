using IMS.Application.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Commands.Create;

public sealed record CreateCategoryCommand(string Name) : IRequest<Result<CategoryDto>>;
