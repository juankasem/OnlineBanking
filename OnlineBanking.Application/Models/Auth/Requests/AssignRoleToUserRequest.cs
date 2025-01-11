

namespace OnlineBanking.Application.Models.Auth.Requests;

public class AssignRoleToUserRequest
{
    public string Email { get; set; }
    public bool IsAdmin { get; set; } = false;
}