
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAppUserAccessor,AppUserAccessor>();

        return services;
    }
}
