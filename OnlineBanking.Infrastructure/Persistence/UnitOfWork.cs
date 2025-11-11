using Microsoft.EntityFrameworkCore.Storage;
using OnlineBanking.Infrastructure.Repositories;

namespace OnlineBanking.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OnlineBankDbContext _dbContext;

    private IDbContextTransaction _dbContextTransaction;

    public IAddressRepository Addresses { get; private set; }

    public IAppUserRepository AppUsers { get; private set; }

    public IBankAccountRepository BankAccounts { get; private set; }

    public IBranchRepository Branches { get; private set; }

    public ICashTransactionsRepository CashTransactions { get; private set; }

    public ICityRepository Cities { get; private set; }

    public ICountryRepository Countries { get; private set; }

    public ICurrencyRepository Currencies { get; private set; }

    public ICustomerRepository Customers { get; private set; }

    public ICustomerAccountRepository CustomerAccounts { get; private set; }

    public ICreditCardsRepository CreditCards { get; private set; }

    public IDebitCardsRepository DebitCards { get; private set; }

    public IDistrictRepository Districts { get; private set; }

    public IFastTransactionsRepository FastTransactions { get; private set; }

    public ILoansRepository Loans { get; private set; }

    public IUtilityPaymentRepository UtilityPayments { get; private set; }


    public UnitOfWork(OnlineBankDbContext dbContext)
    {
        _dbContext = dbContext;

        Addresses ??= new AddressRepository(_dbContext);
        AppUsers ??= new AppUserRepository(_dbContext);
        BankAccounts ??= new BankAccountRepository(_dbContext);
        Branches ??= new BranchRepository(_dbContext);
        CashTransactions ??= new CashTransactionsRepository(_dbContext);
        Cities ??= new CityRepository(_dbContext);
        Countries ??= new CountryRepository(_dbContext);
        Currencies ??= new CurrencyRepository(_dbContext);
        Customers ??= new CustomerRepository(_dbContext);
        CustomerAccounts ??= new CustomerAccountRepository(_dbContext);
        CreditCards ??= new CreditCardsRepository(_dbContext);
        DebitCards ??= new DebitCardsRepository(_dbContext);
        FastTransactions ??= new FastTransactionsRepository(_dbContext);
        Loans ??= new LoansRepository(_dbContext);
        UtilityPayments ??= new UtilityPaymentRepository(_dbContext);
    }

    public IDbContextTransaction CreateDbTransaction() => _dbContext.Database.BeginTransaction();

    public async Task<IDbContextTransaction> CreateDbTransactionAsync() => await _dbContext.Database.BeginTransactionAsync();


    public async Task DisposeAsync() => await _dbContext.DisposeAsync();

    public async Task<int> CompleteDbTransactionAsync()
    {
        using (_dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                var affected = await _dbContext.SaveChangesAsync();
                await _dbContextTransaction.CommitAsync();

                return affected;
            }
            catch
            {
                await _dbContextTransaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task<int> SaveAsync() => await _dbContext.SaveChangesAsync();

    public async void Dispose() => await _dbContext.DisposeAsync();
}
