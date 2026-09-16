using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ProcessTracker.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Log.Error(exception, "An unhandled exception occurred processing the request: {Message}", exception.Message);

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An internal server error occurred.";

        // Map known domain / user-defined exception types to graceful HTTP codes
        switch (exception)
        {
            case ArgumentNullException _:
            case ArgumentException _:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case InvalidOperationException _:
                // We've actively been using InvalidOperationException for business logic conflicts and validation traps
                statusCode = (int)HttpStatusCode.Conflict; 
                message = exception.Message;
                break;
            case KeyNotFoundException _:
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            statusCode,
            message,
            timestamp = DateTime.UtcNow
        });

        return context.Response.WriteAsync(result);
    }
}
