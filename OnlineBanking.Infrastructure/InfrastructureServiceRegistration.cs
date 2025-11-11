using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Infrastructure.Persistence;
using OnlineBanking.Infrastructure.Services;

namespace OnlineBanking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAppUserAccessor, AppUserAccessor>();

        return services;
    }
}