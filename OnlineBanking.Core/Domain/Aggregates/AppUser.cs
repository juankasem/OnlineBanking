using Microsoft.AspNetCore.Identity;

namespace OnlineBanking.Core.Domain.Entities;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public override string PhoneNumber { get; set; }

    private AppUser(string userName, string displayName, string email, string phoneNumber)
    {
        UserName = userName;
        DisplayName = displayName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static AppUser Create(string userName, string displayName, string email, string phoneNumber)
    {
        return new AppUser(userName, displayName, email, phoneNumber);
    }
}