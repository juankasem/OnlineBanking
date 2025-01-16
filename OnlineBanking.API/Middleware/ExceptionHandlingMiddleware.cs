using System.Net;
using OnlineBanking.API.Common;
using OnlineBanking.Application.Models;

namespace OnlineBanking.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

    private static async ValueTask<bool> HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusPhrase = "";
        context.Response.ContentType = "application/json";

        if (ex is FluentValidation.ValidationException fluentException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            statusPhrase = "Validation error(s)";
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            statusPhrase = "Internal Server Error";
        }

        var errorResponse = new ErrorResponse()
        {
            StatusCode = context.Response.StatusCode,
            StatusPhrase = statusPhrase,
        };

        await context.Response.WriteAsJsonAsync(errorResponse);

        return true;
    }
}
