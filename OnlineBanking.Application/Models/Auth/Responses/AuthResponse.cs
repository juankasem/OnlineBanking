
namespace OnlineBanking.Application.Models.Auth.Responses;

public class AuthResponse
{    
    public string Username { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
