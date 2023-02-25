using System.Net;
using OnlineBanking.API.Common;

namespace OnlineBanking.API.Middleware;

public class ExceptionMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next, ILogger logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorResponse = new ErrorResponse()
        {
            StatusCode = context.Response.StatusCode,
            StatusPhrase = "Internal Server Error from custom middleware",
        };

        errorResponse.Errors.Add("Internal Server Error");

        await context.Response.WriteAsync(errorResponse.ToString());
    }
}
