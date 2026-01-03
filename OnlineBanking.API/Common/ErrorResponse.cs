
namespace OnlineBanking.API.Common;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public List<string> Errors { get; } = [];
    public DateTime Timestamp { get; set; } = DateTime.Now;

#nullable enable
    public string? StatusPhrase { get; set; }

    public ErrorResponse()
    {
    }
    private ErrorResponse(int statusCode, string? statusPhrase, List<string> errors)
    {
        StatusCode = statusCode;
        StatusPhrase = statusPhrase;
        Errors = errors;
    }

    public static ErrorResponse Create(int statusCode, string statusPhrase, List<string> errors) =>
    new(statusCode, statusPhrase, errors);
}