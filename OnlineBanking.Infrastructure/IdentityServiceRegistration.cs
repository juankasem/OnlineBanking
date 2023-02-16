using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineBanking.Core.Domain.Entities;
using OnlineBanking.Infrastructure.Persistence;
using OnlineBanking.Infrastructure.Security;

namespace OnlineBanking.Infrastructure;

public static class IdentityServiceRegistration
{
    public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<OnlineBankDbContext>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecretKey"]));
        var tokenValidationParameters = new TokenValidationParameters(){
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidIssuer = configuration["Token:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = tokenValidationParameters;
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
