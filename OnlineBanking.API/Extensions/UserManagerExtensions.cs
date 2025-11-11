global using Microsoft.AspNetCore.Identity;

namespace OnlineBanking.API.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AppUser> FindByEmailFromClaimsPrincipalAsync(this UserManager<AppUser> userManager,
    ClaimsPrincipal user)
    {
        return await userManager.Users.SingleOrDefaultAsync(u => u.Email == user.FindFirstValue(ClaimTypes.Email));
    }
}
