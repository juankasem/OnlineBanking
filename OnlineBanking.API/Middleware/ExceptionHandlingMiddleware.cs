
namespace OnlineBanking.API.Middleware;


public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,
                                          IHostEnvironment environment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // ensure correlation id present for tracing
        var correlationId = context.Request.Headers.ContainsKey("X-Correlation-ID")
            ? context.Request.Headers["X-Correlation-ID"].ToString()
            : Guid.NewGuid().ToString();

        // always return correlation id to client
        context.Response.Headers["X-Correlation-ID"] = correlationId        ;

        // Log full exception (with stack) for diagnostics, include correlation id
        _logger.LogError(exception, "Unhandled exception (CorrelationId: {CorrelationId})", correlationId);

        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow
        };

        var includeDetails = _environment.IsDevelopment();

        // map exception to http status, phrase and list of messages using a modern switch expression
        // Explicitly specify the tuple type to resolve CS8130
        (int httpStatus, string statusPhrase, List<string> messages) = exception switch
        {
            NotValidException ve => (
                StatusCodes.Status400BadRequest,
                "Validation Error(s)",
                ve.ValidationErrors is { Count: > 0 } ? new List<string>(ve.ValidationErrors) :
                    [string.IsNullOrWhiteSpace(ve.Message) ? "Validation failed." : ve.Message]
            ),

            InsufficientFundsException _ => (
                StatusCodes.Status400BadRequest,
                ErrorPhrase.InsufficientFunds,
                new List<string> { "Insufficient funds to complete the operation." }
            ),

            UnauthorizedAccessException _ => (
                StatusCodes.Status403Forbidden,
                ErrorPhrase.Forbidden,
                new List<string> { "You are not authorized to perform this action." }
            ),

            ArgumentNullException ane => (
                StatusCodes.Status400BadRequest,
                ErrorPhrase.BadRequest,
                new List<string> { ane.Message }
            ),

            ArgumentException ae => (
                StatusCodes.Status400BadRequest,
                ErrorPhrase.BadRequest,
                new List<string> { ae.Message }
            ),

            DomainException de => (
             StatusCodes.Status400BadRequest,
                ErrorPhrase.BadRequest,
             new List<string> { de.Message }
            ),

            DbUpdateConcurrencyException dce => (
                StatusCodes.Status409Conflict,
                "Database Concurrency Error",
                BuildDbErrorList(dce, includeDetails)
            ),

            DbUpdateException dbe => (
                StatusCodes.Status500InternalServerError,
                "Database Error",
                BuildDbErrorList(dbe, includeDetails)
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                ErrorPhrase.InternalServerError,
                BuildDefaultErrorList(exception, includeDetails)
            )
        };

        errorResponse.StatusCode = httpStatus;
        errorResponse.StatusPhrase = statusPhrase;

        if (messages is { Count: > 0 })
            errorResponse.Errors.AddRange(messages);

        context.Response.StatusCode = httpStatus;

        // Add correlation id to response for client tracing
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        // Returning true indicates the exception was handled
        return true;
    }

    private static List<string> BuildDbErrorList(DbUpdateException ex, bool includeDetails)
    {
        var list = new List<string> { "A database error occurred while processing the request." };
        if (includeDetails && ex.InnerException is not null)
        {
            list.Add(ex.InnerException.Message);
        }
        return list;
    }

    private static List<string> BuildDefaultErrorList(Exception ex, bool includeDetails)
    {
        var list = new List<string> { "An unexpected error occurred." };
        if (includeDetails)
        {
            list.Add(ex.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                list.Add(ex.StackTrace);
            }
        }
        return list;
    }
}
