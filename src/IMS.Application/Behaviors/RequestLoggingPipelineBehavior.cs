using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using IMS.SharedKernel;

namespace IMS.Application.Behaviors;

// This pipeline behavior logs the request and response of the request it is handling.
public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{

    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling request {Name}", requestName);

        TResponse result = await next();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Request {Name} handled successfully", requestName);
        }

        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                _logger.LogError("Request {Name} failed with error", requestName);
            }
        }

        return result;
    }
}
