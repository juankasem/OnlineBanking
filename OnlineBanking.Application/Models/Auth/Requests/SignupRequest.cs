
namespace OnlineBanking.Application.Models.Auth.Requests;
public class SignupRequest : LoginRequest
{
  public string DisplayName { get; set; }
  public string Email { get; set; }
  public bool IsAdmin { get; set; }
}
