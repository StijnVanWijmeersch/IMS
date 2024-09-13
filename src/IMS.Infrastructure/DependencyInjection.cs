using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using IMS.Application.Abstractions;
using IMS.Application.Caching;
using IMS.Application.Categories.Contracts;
using IMS.Application.Customers.Contracts;
using IMS.Application.Orders.Contracts;
using IMS.Application.Products.Contracts;
using IMS.Application.Statuses.Contracts;
using IMS.Application.Users.Contracts;
using IMS.Infrastructure.Azure;
using IMS.Infrastructure.BackgroundJobs;
using IMS.Infrastructure.Caching;
using IMS.Infrastructure.Categories;
using IMS.Infrastructure.Concretes;
using IMS.Infrastructure.Contexts;
using IMS.Infrastructure.Customers;
using IMS.Infrastructure.Interceptors;
using IMS.Infrastructure.Orders;
using IMS.Infrastructure.Products;
using IMS.Infrastructure.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace IMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<ConvertDomainEventToOutboxMessageInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();

        var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
        var clientId = builder.Configuration.GetSection("KeyVault:ClientId");
        var clientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
        var directoryId = builder.Configuration.GetSection("KeyVault:DirectoryID");

        var credentials = new ClientSecretCredential(directoryId.Value, clientId.Value, clientSecret.Value);
        var uri = new Uri(keyVaultURL.Value!);

        builder.Configuration.AddAzureKeyVault(uri, credentials);

        var client = new SecretClient(uri, credentials);

        services.AddDbContext<IMSDbContext>((sp, options) =>
        {
            var domainEventInterceptor = sp.GetRequiredService<ConvertDomainEventToOutboxMessageInterceptor>();
            var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteInterceptor>();

            var database = client.GetSecret("Database").Value.Value;

            options
            .UseSqlServer(database)
            .AddInterceptors(domainEventInterceptor, softDeleteInterceptor);
        });

        // Register Caching
        services.AddSingleton<ICacheService, CacheService>();

        services
            .AddScoped<IKeyVaultProcessor, KeyVaultProcessor>();

        services
            .AddScoped<IUnitOfWork, UnitOfWork>();

        services
            .AddScoped<IIMSDbContext>(provider => provider.GetRequiredService<IMSDbContext>())
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IStatusRepository, StatusRepository>()
            .AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<SynchronizationContext>();


        services.AddQuartz();

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.ConfigureOptions<OutboxMessageJobSetup>();

        return services;
    }
}
