using IMS.API.Requests.Users;
using IMS.Application.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMS.API.Responses;
using IMS.Application.Users.Commands.Create;

namespace IMS.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users");

        group.MapPost("/", CreateUser);
    }

    private static async Task<IResult> CreateUser([FromBody] CreateUserRequest request, ISender sender)
    {
        var createUserCommand = new CreateUserCommand(request.FirstName, request.LastName, request.Email, request.Password);

        var result = await sender.Send(createUserCommand);

        var response = new ApiResponse<UserDto>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.IsSuccess ? result.Value : null,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Created($"api/users/{result.Value.Id}", response)
            : Results.BadRequest(response);
    }
}
