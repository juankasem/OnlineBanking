
namespace OnlineBanking.Application.Features.CashTransactions.Create.Deposit;

/// <summary>
/// Handles deposit command requests.
/// Validates deposit request, applies domain logic, and persists changes to the account.
/// </summary>
public class MakeDepositCommandHandler(IUnitOfWork uow,
                                        IBankAccountService bankAccountService,
                                        IAppUserAccessor appUserAccessor,
                                        ILogger<MakeDepositCommandHandler> logger) :
                                        IRequestHandler<MakeDepositCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeDepositCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeDepositCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting deposit to IBAN: {IBAN}", request.To);

        var result = new ApiResult<Unit>();

        var iban = request.BaseCashTransaction.IBAN;
        var amountToDeposit = request.BaseCashTransaction.Amount.Value;

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

        if (!ValidateBankAccount(bankAccount, iban, result))
            return result;

        // Prepare deposit transaction
        var recipient = await GetBankAccountOwner(bankAccount);
        var updatedBalance = bankAccount.Balance + amountToDeposit;
        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, recipient, updatedBalance);

        // Apply domain logic
        _bankAccountService.CreateCashTransaction(null, bankAccount, cashTransaction);

        // mark business result as completed on the object before saving so EF persists it in same transaction
        cashTransaction.UpdateStatus(CashTransactionStatus.Completed);

        // Mark aggregate as modified so it will be saved
        _uow.BankAccounts.Update(bankAccount);

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation($"Deposit transaction of Id {cashTransaction.Id} of amount {amountToDeposit} is created");
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError($"Deposit transactyion failed");
        }

        return result;
    }

    #region Validation Methods

    /// <summary>
    /// Validates the deposit request (amount, IBAN presence)
    /// </summary>
    private static bool ValidateDepositRequest(MakeDepositCommand request, ApiResult<Unit> result)
    {
        if (request?.BaseCashTransaction == null)
        {
            result.AddError(ErrorCode.BadRequest, "Invalid deposit request.");
            return false;
        }

        if (request.BaseCashTransaction.Amount.Value <= 0)
        {
            result.AddError(ErrorCode.BadRequest, "Deposit amount must be greater than zero.");
            return false;
        }

        var iban = request.BaseCashTransaction.IBAN;
        if (string.IsNullOrWhiteSpace(iban))
        {
            result.AddError(ErrorCode.BadRequest, "IBAN is required.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates that the bank account exists and is active
    /// </summary>
    private static bool ValidateBankAccount(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount, string iban, ApiResult<Unit> result)
    {
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.BadRequest,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", iban));
            return false;
        }

        if (!bankAccount.IsActive)
        {
            result.AddError(ErrorCode.BadRequest, $"Account with IBAN {iban} is not active.");
            return false;
        }

        return true;
    }

    #endregion
    private async Task<string> GetBankAccountOwner(
    Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount)
    {
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());
        if (loggedInAppUser is null)
        {
            return string.Empty;
        }

        var bankAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        return bankAccountOwner is not null ? bankAccountOwner.FirstName + " " + bankAccountOwner.LastName :
               string.Empty;
    }
}
