
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

        if (!ValidateWithdrawalRequest(request, result))
            return result;

        var iban = request.BaseCashTransaction.IBAN;

        // Retrieve bank account
        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(iban);

        if (!BankAccountHelper.ValidateBankAccount(bankAccount, iban, result))
            return result;

        var amountToWithdraw = decimal.Round(request.BaseCashTransaction.Amount.Value, 2);

        if (!BankAccountHelper.HasSufficientFunds(bankAccount, amountToWithdraw, result))
            return result;

        // Prepare transaction
        var accountOwner = _appUserAccessor.GetDisplayName();
        var updatedBalance = bankAccount.Balance - amountToWithdraw;
        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, accountOwner, updatedBalance);

        // Apply domain logic
        _bankAccountService.CreateCashTransaction(bankAccount, null, cashTransaction);

        // Mark aggregate as modified so it will be saved
        _uow.BankAccounts.Update(bankAccount);

        // mark business result as completed on the object before saving so EF persists it in same transaction
        cashTransaction.UpdateStatus(CashTransactionStatus.Completed);    

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                   "Withdrawal transaction Id: {transactionId} of amount: " +
                   "{amount} from bank account of IBAN: " +
                   "{iban} created successfully.",
                   cashTransaction.Id, 
                   amountToWithdraw, 
                   iban);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError("Withdrawal transaction from IBAN: {IBAN} failed...Please try again", iban);
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

    #endregion
}