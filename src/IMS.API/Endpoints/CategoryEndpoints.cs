using IMS.API.Requests.category;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMS.API.Responses;
using IMS.Application.Categories.Commands.Create;
using IMS.Application.Categories.Commands.Remove;
using IMS.Application.Categories.Commands.Update;
using IMS.Application.Categories.Queries.GetAll;
using IMS.Application.Categories.Queries.GetById;

namespace IMS.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/categories");

        group.MapPost("/", CreateCategory);
        group.MapGet("/", GetAllCategories);
        group.MapGet("/{id}", GetCategoryById).WithName(nameof(GetCategoryById));
        group.MapPut("/{id}", UpdateCategory).WithName(nameof(UpdateCategory));
        group.MapDelete("/{id}", RemoveCategory).WithName(nameof(RemoveCategory));
    }

    public static async Task<IResult> GetAllCategories(int cursor, int pageSize, ISender sender)
    {
        var result = await sender.Send(new GetAllCategoriesQuery(cursor, pageSize));

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.IsSuccess ? result.Value : result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }

    public static async Task<IResult> GetCategoryById(ulong id, ISender sender)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.IsSuccess ? result.Value : result.Error
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }

    public static async Task<IResult> CreateCategory([FromBody] CreateCategoryRequest request, ISender sender)
    {
        var createCommand = new CreateCategoryCommand(request.Name);
        var result = await sender.Send(createCommand);

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.IsSuccess ? result.Value : result.Error,
        };

        return result.IsSuccess
            ? Results.Created($"api/categories/{result.Value.Id}", response)
            : Results.BadRequest(response);
    }

    public static async Task<IResult> UpdateCategory(ulong id, [FromBody] UpdateCategoryRequest request, ISender sender)
    {
        if (id != request.Id)
            return Results.BadRequest("The id in the request body does not match the id in the route.");

        var updateCommand = new UpdateCategoryCommand(id, request.Name);

        var result = await sender.Send(updateCommand);

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 400,
            Result = result.IsSuccess ? result.Value : result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }

    public static async Task<IResult> RemoveCategory(ulong id, ISender sender)
    {
        var result = await sender.Send(new RemoveCategoryCommand(id));

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 204 : 400,
            Result = result.IsSuccess ? result.Value : result.Error.Description,
        };

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(response);
    }
}
