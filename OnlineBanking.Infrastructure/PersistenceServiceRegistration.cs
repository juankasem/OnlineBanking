using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Infrastructure.Repositories;
using OnlineBanking.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace OnlineBanking.Infrastructure;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OnlineBankDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("OnlineBankConnection"));
        });

        services.AddSingleton<IConnectionMultiplexer>(c => {
            var  options = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"));
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<ICashTransactionsRepository, CashTransactionsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFastTransactionsRepository, FastTransactionsRepository>();
        services.AddScoped<ILoansRepository, LoansRepository>();
        services.AddScoped<IUtilityPaymentRepository, UtilityPaymentRepository>();

        return services;
    }
}
