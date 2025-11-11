
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

        var errorResponse = new ErrorResponse();

        if (exception is NotValidException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            statusPhrase = exception.GetType().Name;

            validationException.ValidationErrors.ForEach(er => errorResponse.Errors.Add(er));
        }
        else if (exception is DbUpdateException sqlException)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            statusPhrase = "Sql Exception";
            errorResponse.Errors.Add(exception.InnerException.Message);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            statusPhrase = "Internal Server Error";
            errorResponse.Errors.Add(exception.Message);
        }

        errorResponse.StatusCode = context.Response.StatusCode;
        errorResponse.StatusPhrase = statusPhrase;

        await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}
