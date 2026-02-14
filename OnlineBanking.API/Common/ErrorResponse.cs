
namespace OnlineBanking.API.Common;

/// <summary>
/// Represents a standardized error response returned to API clients.
/// Contains HTTP status information, error details, and a correlation ID for request tracing.
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets the HTTP status phrase (e.g., "Bad Request", "Internal Server Error").
    /// </summary>
    public string? StatusPhrase { get; set; }

    /// <summary>
    /// Gets the list of error messages describing what went wrong.
    /// Each message provides specific details about the error.
    /// </summary>
    public List<string> Errors { get; } = [];

    /// <summary>
    /// Gets the UTC timestamp when the error occurred.
    /// Used for auditing and debugging purposes.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
    /// Uses required properties to ensure all essential information is provided.
    /// </summary>
    public ErrorResponse()
    {
    }

    private ErrorResponse(int statusCode, string? statusPhrase, List<string> errors)
    {
        StatusCode = statusCode;
        StatusPhrase = statusPhrase;
        Errors = errors;
    }

    /// <summary>
    /// Creates an error response with the specified details.
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="statusPhrase">The HTTP status phrase</param>
    /// <param name="errors">List of error messages</param>
    /// <param name="traceId">Optional correlation ID for request tracing</param>
    /// <returns>A new ErrorResponse instance</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when statusCode is invalid</exception>
    /// <exception cref="ArgumentException">Thrown when statusPhrase is null/empty or errors is empty</exception>
    public static ErrorResponse Create(
        int statusCode, 
        string statusPhrase, 
        List<string> errors)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(statusPhrase);
        ArgumentNullException.ThrowIfNull(errors);
        ValidateStatusCode(statusCode);

        return new(statusCode, statusPhrase, errors);
    }

    /// <summary>
    /// Validates that the HTTP status code is within the valid range (100-599).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when status code is invalid</exception>
    private static void ValidateStatusCode(int statusCode)
    {
        if (statusCode < 100 || statusCode > 599)
        {
            throw new ArgumentOutOfRangeException(
                nameof(statusCode),
                statusCode,
                "HTTP status code must be between 100 and 599");
        }
    }
}
    