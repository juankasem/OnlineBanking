
namespace OnlineBanking.Application.Models.Auth.Requests;

#nullable enable
public class RefreshTokenRequest
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
