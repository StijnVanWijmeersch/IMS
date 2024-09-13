using IMS.API.Requests.Customer;
using IMS.Application.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMS.API.Responses;
using IMS.Application.Customers.Commands.Create;
using IMS.Application.Customers.Queries.GetAll;

namespace IMS.API.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customers");

        group.MapPost("/", CreateCustomer);
        group.MapGet("/", GetAllCustomers);
        //group.MapGet("/{id}", GetCustomerById);
        //group.MapPut("/{id}", UpdateCustomer);
        //group.MapDelete("/{id}", DeleteCustomer);
    }

    public static async Task<IResult> GetAllCustomers(ISender sender)
    {
        var queryResult = await sender.Send(new GetAllCustomersQuery());

        var response = new ApiResponse<IEnumerable<CustomerDto>>
        {
            Succeeded = queryResult.IsSuccess,
            StatusCode = queryResult.IsSuccess ? 200 : 400,
            Result = queryResult.Value,
            ErrorMessage = queryResult.Error.Description
        };

        return queryResult.IsSuccess
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }

    public static async Task<IResult> CreateCustomer([FromBody] CreateCustomerRequest request, ISender sender)
    {
        var createCommand = new CreateCustomerCommand(request.FirstName, request.LastName, request.Email);
        var result = await sender.Send(createCommand);

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.IsSuccess ? result.Value : result.Error
        };

        return result.IsSuccess
            ? Results.Created($"api/customers/{result.Value.Id}", response)
            : Results.BadRequest(response);

    }


}
