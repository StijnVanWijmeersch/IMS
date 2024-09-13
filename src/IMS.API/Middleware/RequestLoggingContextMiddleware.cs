using Serilog.Context;

namespace IMS.API.Middleware;

// This middleware is used to log the state of the request pipeline which handles the current request.s
public class RequestLoggingContextMiddleware
{

    private readonly RequestDelegate _next;

    public RequestLoggingContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorralationId", context.TraceIdentifier))
        {
            return _next(context);
        }
    }
}
