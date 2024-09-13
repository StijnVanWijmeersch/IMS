using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Categories.Commands.Remove;

public sealed record RemoveCategoryCommand(ulong Id) : IRequest<Result<CategoryDto>>;
