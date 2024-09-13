using IMS.API.Requests.Products;
using IMS.Application.Products;
using IMS.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMS.API.Responses;
using IMS.Application.Products.Commands.Create;
using IMS.Application.Products.Commands.Remove;
using IMS.Application.Products.Commands.Update;
using IMS.Application.Products.Queries.GetAll;
using IMS.Application.Products.Queries.GetAllByCategory;
using IMS.Application.Products.Queries.GetAllBySearch;
using IMS.Application.Products.Queries.GetAllFromStock;
using IMS.Application.Products.Queries.GetAllSold;
using IMS.Application.Products.Queries.GetById;

namespace IMS.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products");

        group.MapPost("/", CreateProduct);
        group.MapGet("/", GetAllProducts);
        group.MapGet("/sold", GetAllSoldProducts);
        group.MapGet("/stock", GetAllProductsFromStock);
        group.MapGet("/{id}", GetProductById);
        group.MapGet("/category/{categoryid}", GetProductByCategoryId);
        group.MapGet("/search/{search}", GetProductsByName);
        group.MapGet("/search/stock/{search}", GetProductsInStockByName); ;
        group.MapPut("/{id}", UpdateProduct);
        group.MapDelete("/{id}", RemoveProduct);
    }

    private static async Task<IResult> GetAllSoldProducts(ISender sender)
    {
        var result = await sender.Send(new GetAllSoldProductsQuery());

        var response = new ApiResponse<int>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.IsSuccess ? result.Value : 0,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }
    private static async Task<IResult> RemoveProduct(ulong id, ISender sender)
    {
        var result = await sender.Send(new RemoveProductCommand(id));

        var response = new ApiResponse<ProductDto>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 204 : 400,
            Result = result.IsSuccess ? result.Value : null,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(response);
    }
    private static async Task<IResult> GetProductByCategoryId(ulong categoryId, int cursor, int pageSize, ISender sender)
    {
        Result<Page<ProductDto>> result;

        if (categoryId == 0)
        {
            result = await sender.Send(new GetAllProductsQuery(cursor, pageSize));
        }
        else
        {
            result = await sender.Send(new GetAllProductsByCategoryQuery(cursor, pageSize, categoryId));
        }

        var response = new ApiResponse<Page<ProductDto>>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }
    private static async Task<IResult> UpdateProduct(ulong id, [FromBody] UpsertProductRequest request, ISender sender)
    {
        var updateCommand = new UpdateProductCommand(
           id, request.Name, request.Description,
           request.ShortDescription,
           request.Sku, request.InStock, request.StockQuantity,
           request.Price, request.Image, request.Categories, request.Stores);

        var result = await sender.Send(updateCommand);

        var response = new ApiResponse<ProductDto>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(response);
    }
    private static async Task<IResult> GetProductsInStockByName(string search, ISender sender)
    {
        var result = await sender.Send(new GetAllProductsBySearchQuery(search, true));

        var response = new ApiResponse<IEnumerable<ProductDto>>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }
    private static async Task<IResult> GetAllProductsFromStock(int cursor, int pageSize, ISender sender)
    {
        var result = await sender.Send(new GetAllProductsFromStockQuery(cursor, pageSize));

        var response = new ApiResponse<Page<ProductDto>>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);

    }
    private static async Task<IResult> GetProductsByName(string search, ISender sender)
    {
        var result = await sender.Send(new GetAllProductsBySearchQuery(search));

        var response = new ApiResponse<IEnumerable<ProductDto>>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }
    public static async Task<IResult> GetProductById(ulong id, ISender sender)
    {
        var result = await sender.Send(new GetProductByIdQuery(id));

        var response = new ApiResponse<ProductDto>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }
    public static async Task<IResult> GetAllProducts(int cursor, int pageSize, ISender sender)
    {
        var result = await sender.Send(new GetAllProductsQuery(cursor, pageSize));

        var response = new ApiResponse<Page<ProductDto>>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }
    public static async Task<IResult> CreateProduct([FromBody] UpsertProductRequest request, ISender sender)
    {
        var createCommand = new CreateProductCommand(
            request.Name, request.Description, request.ShortDescription,
            request.Sku, request.InStock, request.StockQuantity,
            request.Price, request.Image, request.Categories, request.Stores);

        var result = await sender.Send(createCommand);

        var response = new ApiResponse<ProductDto>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Created($"api/products/{result.Value.Id}", response)
            : Results.BadRequest(response);
    }
}
