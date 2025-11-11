using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineBanking.Application.Common.Behaviors;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Mappings.Branches;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Mappings.CreditCards;
using System.Reflection;

namespace OnlineBanking.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IBankAccountMapper, BankAccountMapper>();
        services.AddScoped<IBranchMapper, BranchMapper>();
        services.AddScoped<ICashTransactionsMapper, CashTransactionsMapper>();
        services.AddScoped<ICreditCardsMapper, CreditCardsMapper>();

        services.AddScoped<IBankAccountService, BankAccountService>();

        return services;
    }
}