
namespace OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;

/// <summary>
/// Handles the withdrawal command request
/// Validates the withdrawal request, applies the domain logic, and persists changes
/// </summary>
public class MakeWithdrawalCommandHandler(IUnitOfWork uow,
                                            IBankAccountService bankAccountService,
                                            IAppUserAccessor appUserAccessor,
                                            ILogger<MakeWithdrawalCommandHandler> logger) :
                                            IRequestHandler<MakeWithdrawalCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeWithdrawalCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeWithdrawalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start creating withdrawal from {from}", request.From);

        var result = new ApiResult<Unit>();

        // Validate request
        if (!ValidateWithdrawalRequest(request, result))
            return result;

        var iban = request.BaseCashTransaction.IBAN;

        // Retrieve bank account
        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(iban);
        var amountToWithdraw = decimal.Round(request.BaseCashTransaction.Amount.Value, 2);

        if (!ValidateBankAccount(bankAccount, iban, result))
            return result;


        if (!HasSufficientFunds(bankAccount, amountToWithdraw, result))
            return result;
        

        // Prepare transaction
        var bankAccountOwnerName = await GetBankAccountOwner(bankAccount);
        var updatedBalance = bankAccount.Balance - amountToWithdraw;
        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, bankAccountOwnerName, updatedBalance);

        // Apply domain logic
        _bankAccountService.CreateCashTransaction(bankAccount, null, cashTransaction);

        // mark business result as completed on the object before saving so EF persists it in same transaction
        cashTransaction.UpdateStatus(CashTransactionStatus.Completed);

        // Mark aggregate as modified so it will be saved
        _uow.BankAccounts.Update(bankAccount);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                   "Withdrawal transaction {TransactionId} of amount {Amount} from IBAN {IBAN} created successfully.",
                   cashTransaction.Id, amountToWithdraw, iban);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError("Withdrawal transaction from IBAN {IBAN} failed to commit.", iban);
        }

        return result;
    }

    #region Validation Methods

    /// <summary>
    /// Validates the withdrawal request (amount, IBAN presence)
    /// </summary>
    private static bool ValidateWithdrawalRequest(MakeWithdrawalCommand request, ApiResult<Unit> result)
    {
        if (request?.BaseCashTransaction == null)
        {
            result.AddError(ErrorCode.BadRequest, "Invalid withdrawal request.");
            return false;
        }

        if (request.BaseCashTransaction.Amount.Value <= 0)
        {
            result.AddError(ErrorCode.BadRequest, "Withdrawal amount must be greater than zero.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates that the bank account exists
    /// </summary>
    private static bool ValidateBankAccount(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount,
                                          string iban,
                                          ApiResult<Unit> result)
    {
        var success = true;
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.BadRequest, $"Sender account with IBAN {iban} not found.");
            success = false;
        }

        return success;
    }

    /// <summary>
    /// Validates that the account has sufficient funds for withdrawal
    /// </summary>
    private static bool HasSufficientFunds(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount, decimal totalAmount, ApiResult<Unit> result)
    {
        if (bankAccount.AllowedBalanceToUse < totalAmount)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);
            return false;
        }

        return true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Validates that the bank account exists
    /// </summary>
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

    #endregion
}