using IMS.API.Endpoints;
using IMS.API.Middleware;
using IMS.Application;
using IMS.Infrastructure;
using Microsoft.Extensions.Logging.AzureAppServices;
using Serilog;

namespace IMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Logging For Azure App Services
            builder.Logging.AddAzureWebAppDiagnostics();

            builder.Services.Configure<AzureBlobLoggerOptions>(config =>
            {
                config.BlobName = "logs.txt";
            });


            // Configure Serilog For Development
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            });


            builder.Services.AddAuthorization();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder);

            var app = builder.Build();

            app.MapCategoryEndpoints();
            app.MapOrderEndpoints();
            app.MapCustomerEndpoints();
            app.MapStatusEndpoints();
            app.MapProductEndpoints();
            app.MapUserEndpoints();

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestLoggingContextMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseAuthorization();

            app.Run();
        }
    }
}
