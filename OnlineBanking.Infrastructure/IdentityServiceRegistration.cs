using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineBanking.Infrastructure.Security;

namespace OnlineBanking.Infrastructure;

public static class IdentityServiceRegistration
{
    public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
        var tokenValidationParameters = new TokenValidationParameters(){
            ValidateIssuerSigningKey =true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => 
                {
                    opt.TokenValidationParameters = tokenValidationParameters;
                });

        services.AddAuthorization(opt => {
            opt.AddPolicy("IsAccountOwner", policy => 
            {
                policy.Requirements.Add(new IsAccountOwnerRequirement());
            });
        });
        services.AddTransient<IAuthorizationHandler, IsAccountOwnerRequirementHandler>();

        return services;
    }
}
