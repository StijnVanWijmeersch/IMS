using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Commands.Remove;

public sealed record RemoveProductCommand(ulong ProductId) : IRequest<Result<ProductDto>>;
