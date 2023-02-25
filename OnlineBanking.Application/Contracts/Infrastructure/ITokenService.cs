using System.Security.Claims;

namespace OnlineBanking.Application.Contracts.Infrastructure;

public interface ITokenService
{
    string CreateToken(List<Claim> authClaims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}