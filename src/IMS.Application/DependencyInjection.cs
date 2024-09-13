using IMS.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddMediatR((config) =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        return services;
    }
}
