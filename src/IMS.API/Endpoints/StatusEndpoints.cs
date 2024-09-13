using IMS.API.Requests.Statuses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMS.API.Responses;
using IMS.Application.Statuses.Commands.Create;

namespace IMS.API.Endpoints;

public static class StatusEndpoints
{
    public static void MapStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/statuses");

        group.MapPost("/", CreateStatus);
    }

    public static async Task<IResult> CreateStatus([FromBody] CreateStatusRequest request, ISender sender)
    {
        var createCommand = new CreateStatusCommand(request.Name);
        var result = await sender.Send(createCommand);

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.IsSuccess ? result.Value : result.Error
        };

        return result.IsSuccess
            ? Results.Created($"api/statuses/{result.Value.Id}", response)
            : Results.BadRequest(response);
    }
}
