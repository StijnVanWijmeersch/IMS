using IMS.Application.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Customers;
using IMS.Domain.JoinTables;
using IMS.Domain.Orders;
using IMS.Domain.Outbox;
using IMS.Domain.Products;
using IMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Contexts;

public class IMSDbContext : DbContext, IIMSDbContext
{

    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }

    // Join tables
    public DbSet<OrderProducts> OrderProducts { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    public IMSDbContext(DbContextOptions<IMSDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IMSDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
