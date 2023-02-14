using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineBanking.Application.Features.BankAccount.Validators;
using OnlineBanking.Application.Mappings.Addresses;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Mappings.CashTransactions;

namespace OnlineBanking.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(typeof(CreateBankAccountRequestValidator).Assembly);

        services.AddScoped<IBankAccountMapper, BankAccountMapper>();
        services.AddScoped<ICashTransactionsMapper, CashTransactionsMapper>();


        return services;
    }
}
