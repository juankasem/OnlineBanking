using Microsoft.AspNetCore.Diagnostics;
using OnlineBanking.API.Common;

namespace OnlineBanking.API.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);

        string statusPhrase;
        context.Response.ContentType = "application/json";

        if (exception is FluentValidation.ValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            statusPhrase = exception.GetType().Name;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            statusPhrase = "Internal Server Error";
        }

        var errorResponse = new ErrorResponse()
        {
            StatusCode = context.Response.StatusCode,
            StatusPhrase = statusPhrase,
        };

        errorResponse.Errors.Add(exception.Message);

        await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;    
    }
}
