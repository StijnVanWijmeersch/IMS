using IMS.Application.Categories;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Commands.Update;

public sealed record UpdateCategoryCommand(ulong Id, string Name) : IRequest<Result<CategoryDto>>;
