using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OnlineBanking.Application.Common.Behaviors;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Mappings.Branches;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Mappings.CreditCards;
using OnlineBanking.Core.Domain.Services.BankAccount;

namespace OnlineBanking.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IBankAccountMapper, BankAccountMapper>();
        services.AddScoped<IBranchMapper, BranchMapper>();
        services.AddScoped<ICashTransactionsMapper, CashTransactionsMapper>();
        services.AddScoped<ICreditCardsMapper, CreditCardsMapper>();

        services.AddScoped<IBankAccountService, BankAccountService>();

        return services;
    }
}
