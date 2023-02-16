using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.API.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AppUser> FindByEmailFromClaimsPrincipalAsync(this UserManager<AppUser> userManager,
    ClaimsPrincipal user)
    {
        return await userManager.Users.SingleOrDefaultAsync(u => u.Email == user.FindFirstValue(ClaimTypes.Email));
    }
}
