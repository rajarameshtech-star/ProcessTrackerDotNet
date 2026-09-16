using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ProcessTracker.API.Middlewares;

public class GlobalLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
            sw.Stop();
            
            // Only log non-exception requests here (exceptions will be caught in the catch block)
            var statusCode = context.Response.StatusCode;
            if (statusCode >= 400 && statusCode < 500)
            {
                Log.Warning("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms",
                    context.Request.Method, context.Request.Path, statusCode, sw.ElapsedMilliseconds);
            }
            else
            {
                Log.Information("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms",
                    context.Request.Method, context.Request.Path, statusCode, sw.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            Log.Error(ex, "HTTP {RequestMethod} {RequestPath} failed in {Elapsed} ms with Exception: {ExceptionMessage}",
                context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds, ex.Message);
            
            // Re-throw to allow standard error handling (or handle uniquely here)
            throw;
        }
    }
}
