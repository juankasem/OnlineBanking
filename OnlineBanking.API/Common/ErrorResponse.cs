
namespace OnlineBanking.API.Common;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public List<string> Errors { get; } = new List<string>();
    public DateTime Timestamp { get; set; }

#nullable enable
    public string? StatusPhrase { get; set; }
}