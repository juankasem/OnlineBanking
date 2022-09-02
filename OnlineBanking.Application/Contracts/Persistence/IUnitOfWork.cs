using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IUnitOfWork : IDisposable
{
    IAddressRepository Addresses { get; }

    IAppUserRepository AppUsers { get; }

    IBankAccountRepository BankAccounts { get; }

    IBranchRepository Branches { get; }

    ICashTransactionsRepository CashTransactions { get; }

    ICityRepository Cities { get; }

    ICountryRepository Countries { get; }

    ICurrencyRepository Currencies { get; }

    ICustomerRepository Customers { get; }

    ICreditCardsRepository CreditCards { get; }

    IDebitCardsRepository DebitCards { get; }

    IDistrictRepository Districts { get; }

    IFastTransactionsRepository FastTransactions { get; }

    ILoansRepository Loans { get; }

    IUtilityPaymentRepository UtilityPayments { get; }

    IDbContextTransaction CreateDbTransaction();
    Task<IDbContextTransaction> CreateDbTransactionAsync();
    Task<int> SaveAsync();
    Task<int> CompleteTransactionAsync();
}
