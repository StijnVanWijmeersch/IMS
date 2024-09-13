using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Products.Commands.Update;

public sealed record UpdateProductCommand(
    ulong Id,
    string Name,
    string? Description,
    string? ShortDescription,
    string Sku,
    bool InStock,
    int StockQuantity,
    decimal Price,
    string? ImageUrl,
    List<ulong> Categories,
    List<ulong> Stores) : IRequest<Result<ProductDto>>;
