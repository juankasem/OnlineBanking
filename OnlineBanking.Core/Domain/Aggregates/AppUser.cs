using Microsoft.AspNetCore.Identity;

namespace OnlineBanking.Core.Domain.Entities;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }
}
