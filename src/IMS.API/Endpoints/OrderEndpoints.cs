using IMS.API.Requests.Order;
using IMS.Application.Orders;
using IMS.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMS.API.Responses;
using IMS.Application.Orders.Commands.Create;
using IMS.Application.Orders.Queries.GetAll;
using IMS.Application.Orders.Queries.GetById;
using IMS.Application.Orders.Queries.GetLatest;
using IMS.Application.Orders.Queries.GetTotalCount;

namespace IMS.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/orders");

        group.MapPost("/", CreateOrder);
        group.MapGet("/", GetAllOrders);
        group.MapGet("/count", GetAllCompletedOrdersCount);
        group.MapGet("/latest/{amount}", GetLatestOrders);
        group.MapGet("/{id}", GetOrderById);
        //group.MapPut("/{id}", UpdateOrder);
        //group.MapDelete("/{id}", RemoveOrder);
    }

    private static async Task<IResult> GetAllCompletedOrdersCount(ISender sender)
    {
        var result = await sender.Send(new GetTotalOrderCountQuery());

        var response = new ApiResponse<int>
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

    public static async Task<IResult> GetLatestOrders(int amount, ISender sender)
    {
        var result = await sender.Send(new GetLatestOrdersQuery(amount));

        var response = new ApiResponse<IEnumerable<OrderDto>>
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
    public static async Task<IResult> CreateOrder([FromBody] CreateOrderRequest request, ISender sender)
    {
        var createCommand = new CreateOrderCommand(
            request.CustomerId,
            request.StatusId,
            request.Products,
            request.Invoices);

        var result = await sender.Send(createCommand);

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 201 : 400,
            Result = result.IsSuccess ? result.Value : result.Error
        };

        return result.IsSuccess
            ? Results.Created($"api/orders/{result.Value.Id}", response)
            : Results.BadRequest(response);
    }
    public static async Task<IResult> GetOrderById(ulong id, ISender sender)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id));

        var response = new ApiResponse
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 404,
            Result = result.IsSuccess ? result.Value : result.Error
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.NotFound(response);
    }
    public static async Task<IResult> GetAllOrders(int cursor, int pageSize, ISender sender)
    {
        var result = await sender.Send(new GetAllOrdersQuery(cursor, pageSize));

        var response = new ApiResponse<Page<OrderDto>>
        {
            Succeeded = result.IsSuccess,
            StatusCode = result.IsSuccess ? 200 : 404,
            Result = result.Value,
            ErrorMessage = result.Error.Description,
        };

        return result.IsSuccess
            ? Results.Ok(response)
            : Results.NotFound(response);
    }

}
