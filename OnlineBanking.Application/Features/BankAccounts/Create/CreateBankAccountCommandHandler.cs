
namespace OnlineBanking.Application.Features.BankAccounts.Create;

/// <summary>
/// Handles bank account creation requests.
/// Validates account uniqueness, retrieves and associates customers, and persists the new account.
/// </summary>
/// <remarks>
/// Initializes a new instance of the handler.
/// </remarks>
public class CreateBankAccountCommandHandler(
    IUnitOfWork uow, 
    ILogger<MakeDepositCommandHandler> logger) : 
    IRequestHandler<CreateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<MakeDepositCommandHandler> _logger = logger;

    /// <summary>
    /// Handles the bank account creation request.
    /// </summary>
    public async Task<ApiResult<Unit>> Handle(
        CreateBankAccountCommand request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = new ApiResult<Unit>();
        var accountNo = request.AccountNo;

        _logger.LogInformation(
                  "Processing bank account creation: AccountNo={AccountNo}, " +
                  "Type={Type}, " +
                  "CurrencyId={CurrencyId}",
                  accountNo,
                  request.Type,
                  request.CurrencyId);

        if (await _uow.BankAccounts.ExistsAsync(accountNo))
        {
            result.AddError(ErrorCode.CustomerAlreadyExists,
            string.Format(BankAccountErrorMessages.AlreadyExists, accountNo));
            return result;
        }

        // Create bank account aggregate
        var bankAccount = CreateBankAccount(request);

        // Retrieve and associate customers
        if (!await AssociateCustomersWithAccountAsync(bankAccount, 
            request.CustomerNos, 
            result, 
            cancellationToken))
        {
            return result;
        }

        // Persist changes
        await _uow.BankAccounts.AddAsync(bankAccount);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                "Bank account created successfully - Id: {BankAccountId}, " +
                "AccountNo: {AccountNo}, " +
                "IBAN: {IBAN}," +
                " Type: {Type}, " +
                "Balance: {Balance}, " +
                "Currency: {CurrencyId}, " +
                "Branch: {BranchId}",
                bankAccount.Id,
                bankAccount.AccountNo,
                bankAccount.IBAN,
                bankAccount.Type,
                bankAccount.Balance,
                bankAccount.CurrencyId,
                bankAccount.BranchId);
        }
        else
        {
            _logger.LogError(
                "Failed to persist bank account creation for account number: {AccountNo}. " +
                "Database transaction returned 0 rows affected",
                accountNo);
            result.AddError(ErrorCode.UnknownError, BankAccountErrorMessages.Unknown);
        }

        return result;
    }

    #region Private helper methods
    private static BankAccount CreateBankAccount(CreateBankAccountCommand request) =>
        BankAccount.Create(request.AccountNo, 
                    request.IBAN, 
                    request.Type,
                    request.BranchId, 
                    request.Balance,
                    request.AllowedBalanceToUse, 
                    request.MinimumAllowedBalance,
                    request.Debt, 
                    request.CurrencyId);

    /// <summary>
    /// Retrieves customers and associates them with the bank account.
    /// </summary>
    private async Task<bool> AssociateCustomersWithAccountAsync(
        BankAccount bankAccount,
        string[] customerNos,
        ApiResult<Unit> result,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        ArgumentNullException.ThrowIfNull(customerNos);

        if (customerNos.Length == 0)
        {
            _logger.LogWarning("Bank account creation: No customers provided for account {AccountNo}", bankAccount.AccountNo);
            return true; // Account can be created without owners
        }

        foreach (var customerNo in customerNos)
        {
            var customer = await _uow.Customers.GetByCustomerNoAsync(customerNo);

            if (customer is null)
            {
                _logger.LogError(
                    "Bank account creation failed: Customer {CustomerNo} not found for account {AccountNo}",
                    customerNo,
                    bankAccount.AccountNo);

                result.AddError(ErrorCode.NotFound, string.Format(
                    CustomerErrorMessages.NotFound,
                    "Customer No",
                     customerNo));
                return false;
            }

            // Create and add the customer-bank account relationship
            var bankAccountOwner = CustomerBankAccount.Create(bankAccount.Id, customer.Id);
            bankAccount.AddOwnerToBankAccount(bankAccountOwner);

            _logger.LogDebug(
                "Associated customer {CustomerNo} with bank account {AccountNo}",
                customerNo,
                bankAccount.AccountNo);
        }

        return true;
    }

    #endregion
}
