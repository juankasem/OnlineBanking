
namespace OnlineBanking.API.Common;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public List<string> Errors { get; } = [];
    public DateTime Timestamp { get; set; } = DateTime.Now;

#nullable enable
    public string? StatusPhrase { get; set; }
}