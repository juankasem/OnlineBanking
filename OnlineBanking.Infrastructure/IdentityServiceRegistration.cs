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
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 5;
        })
            .AddEntityFrameworkStores<OnlineBankDbContext>()
            .AddDefaultTokenProviders();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
        var tokenValidationParameters = new TokenValidationParameters(){
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateIssuer = false,
            ValidateAudience = false
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });

        services.AddAuthorization(
            opt => {
            opt.AddPolicy("IsAccountOwner", policy => 
            {
                policy.Requirements.Add(new IsAccountOwnerRequirement());
            });
        }
        );
        services.AddTransient<IAuthorizationHandler, IsAccountOwnerRequirementHandler>();

        return services;
    }
}
