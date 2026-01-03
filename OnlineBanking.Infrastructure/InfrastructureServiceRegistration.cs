using Azure.Identity;
using Azure.Messaging.ServiceBus;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Infrastructure.Consumers;
using OnlineBanking.Infrastructure.Persistence;
using OnlineBanking.Infrastructure.Services;

namespace OnlineBanking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Add Azure Key Vault (KeyVaultUri stored in an env var or config)
        var keyVaultUri = configuration["KeyVault:VaultUri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        // Register services that read configuration (ServiceBusClient example)
        var sbConn = configuration.GetConnectionString("azure-service-bus");
        if (!string.IsNullOrEmpty(sbConn))
        {
            services.AddSingleton(_ => new ServiceBusClient(sbConn));
        }
        else
        {
            // Optionally use managed identity + fully-qualified namespace:
            var fqns = configuration["ServiceBus:FullyQualifiedNamespace"];
            if (!string.IsNullOrEmpty(fqns))
            {
                services.AddSingleton(_ => new ServiceBusClient(fqns, new DefaultAzureCredential()));
            }
        }

        services.Configure<ServiceBusOptions>(configuration.GetSection("ServiceBusOptions"));
        services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();
        services.AddHostedService<CashTransactionCreatedEventConsumer>();
        services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAppUserAccessor, AppUserAccessor>();

        return services;
    }
}