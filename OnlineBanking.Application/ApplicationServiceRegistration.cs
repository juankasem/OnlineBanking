using OnlineBanking.Application.Common.Behaviors;

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
        services.AddScoped<IBankAccountHelper, BankAccountHelper>();

        return services;
    }
}