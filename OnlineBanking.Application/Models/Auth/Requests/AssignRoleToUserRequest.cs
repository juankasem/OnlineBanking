

namespace OnlineBanking.Application.Models.Auth.Requests;

public class AssignRoleToUserRequest
{
    public string Email { get; set; }
    public string RoleName { get; set; } 
}