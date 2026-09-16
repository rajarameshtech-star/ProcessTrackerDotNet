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
        
        await _next(context);
        
        sw.Stop();
        
        var statusCode = context.Response.StatusCode;
        if (statusCode >= 400 && statusCode < 500)
        {
            Log.Warning("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms",
                context.Request.Method, context.Request.Path, statusCode, sw.ElapsedMilliseconds);
        }
        else if (statusCode >= 500)
        {
            Log.Error("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms",
                context.Request.Method, context.Request.Path, statusCode, sw.ElapsedMilliseconds);
        }
        else
        {
            Log.Information("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms",
                context.Request.Method, context.Request.Path, statusCode, sw.ElapsedMilliseconds);
        }
    }
}
