
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineBanking.Infrastructure.Persistence;
using OnlineBanking.Infrastructure.Persistence.Interceptors;
using OnlineBanking.Infrastructure.Repositories;
using StackExchange.Redis;

namespace OnlineBanking.Infrastructure;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OnlineBankDbContext>((sp,options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(configuration.GetConnectionString("OnlineBankingConnection"));
        });

        services.AddSingleton<IConnectionMultiplexer>(c =>
        {
            var options = ConfigurationOptions.Parse(configuration?.GetConnectionString("Redis")!);
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<ICashTransactionsRepository, CashTransactionsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFastTransactionsRepository, FastTransactionsRepository>();
        services.AddScoped<ILoansRepository, LoansRepository>();
        services.AddScoped<IUtilityPaymentRepository, UtilityPaymentRepository>();

        services.AddScoped<ISaveChangesInterceptor, AuditableEntitiesInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();

        return services;
    }
}
