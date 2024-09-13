using IMS.Domain.Categories;
using IMS.Domain.Customers;
using IMS.Domain.JoinTables;
using IMS.Domain.Orders;
using IMS.Domain.Outbox;
using IMS.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Abstractions;

// This is a marker interface for dependency injection
public interface IIMSDbContext
{
    public DbSet<Product> Products { get; }
    public DbSet<Category> Categories { get; }
    public DbSet<Customer> Customers { get; }
    public DbSet<ProductCategory> ProductCategories { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<OutboxMessage> OutboxMessages { get; }
    public DbSet<OrderProducts> OrderProducts { get; set; }
}
