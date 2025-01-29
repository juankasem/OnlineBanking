namespace OnlineBanking.Application.Models.Auth.Requests;

public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}