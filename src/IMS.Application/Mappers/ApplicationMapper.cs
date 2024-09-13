using IMS.Application.Categories;
using IMS.Application.Customers;
using IMS.Application.Orders;
using IMS.Application.Products;
using IMS.Application.Statuses;
using IMS.Application.Users;
using IMS.Domain.Categories;
using IMS.Domain.Customers;
using IMS.Domain.Orders;
using IMS.Domain.Products;
using IMS.Domain.Users;

namespace IMS.Application.Mappers;

internal static class ApplicationMapper
{
    public static ProductDto ToProductDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name.Value,
            Sku = product.Sku.Value,
            InStock = product.InStock,
            Image = product.Image.Value,
            StockQuantity = product.StockQuantity,
            Price = product.Price.Value,
            Date = product.CreatedAt,
        };
    }

    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name.Value,
        };
    }

    public static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            PlacedAt = order.CreatedAt
        };
    }

    public static CustomerDto ToCustomerDto(this Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName.Value,
            LastName = customer.LastName.Value,
            Email = customer.Email.Value,
        };
    }

    public static StatusDto ToStatusDto(this Status status)
    {
        return new StatusDto
        {
            Id = status.Id,
            Name = status.Name.Value
        };
    }

    public static UserDto ToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value
        };
    }

}
