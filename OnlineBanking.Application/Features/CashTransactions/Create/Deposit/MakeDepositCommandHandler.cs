
namespace OnlineBanking.Application.Features.CashTransactions.Create.Deposit;

/// <summary>
/// Handles deposit command requests.
/// Validates deposit request, applies domain logic, and persists changes to the account.
/// </summary>
public class MakeDepositCommandHandler(IUnitOfWork uow,
    IBankAccountService bankAccountService,
    IAppUserAccessor appUserAccessor,
    IBankAccountHelper bankAccountHelper,
    ILogger<MakeDepositCommandHandler> logger) :
    IRequestHandler<MakeDepositCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly IBankAccountHelper _bankAccountHelper = bankAccountHelper;
    private readonly ILogger<MakeDepositCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeDepositCommand request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var result = new ApiResult<Unit>();
        var iban = request.To;

        _logger.LogInformation("Starting deposit to IBAN: {IBAN}", iban);

        if (!ValidateDepositRequest(request, result))
            return result;

        var amountToDeposit = request.BaseCashTransaction.Amount.Value;
        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(iban);

        if (!_bankAccountHelper.ValidateBankAccount(bankAccount, iban, result))
            return result;

        // Prepare deposit transaction
        var recipient = _appUserAccessor.GetDisplayName();
        var updatedBalance = bankAccount.Balance + amountToDeposit;
        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, recipient, updatedBalance);

        // Apply domain logic 
        _bankAccountService.CreateCashTransaction(senderAccount: null, bankAccount, cashTransaction);

        // Mark aggregate as modified so it will be saved
        _uow.BankAccounts.Update(bankAccount);

        // mark business result as completed on the object before saving so EF persists it in same transaction
        cashTransaction.UpdateStatus(CashTransactionStatus.Completed);

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                  "Deposit transaction Id: {transactionId} of amount: " +
                  "{amount} from bank account of IBAN: " +
                  "{iban} created successfully.",
                  cashTransaction.Id,
                  amountToDeposit,
                  iban);
        }
        else
        {
            _logger.LogError("Failed to persist deposit transaction for IBAN: {IBAN}", iban);
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
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

    #endregion
}
