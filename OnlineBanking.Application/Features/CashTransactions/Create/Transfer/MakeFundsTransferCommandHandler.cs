
namespace OnlineBanking.Application.Features.CashTransactions.Create.Transfer;

/// <summary>
/// Handles fund transfer command requests.
/// Validates transfer request, applies domain logic, and persists changes to both accounts.
/// </summary>
public class MakeFundsTransferCommandHandler(IUnitOfWork uow,
    IBankAccountService bankAccountService,
    IAppUserAccessor appUserAccessor,
    IBankAccountHelper bankAccountHelper,
    ILogger<MakeFundsTransferCommandHandler> logger) :
    IRequestHandler<MakeFundsTransferCommand, ApiResult<Unit>>
{
    private const decimal TransferFeePercentage = 0.025M;
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly IBankAccountHelper _bankAccountHelper = bankAccountHelper;
    private readonly ILogger<MakeFundsTransferCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeFundsTransferCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var result = new ApiResult<Unit>();
        var senderIBAN = request.From;
        var recipientIBAN = request.To;

        _logger.LogInformation(
            "Processing fund transfer: From={SenderIBAN}, To={RecipientIBAN}, Amount={Amount}",
            senderIBAN,
            recipientIBAN,
            request.BaseCashTransaction.Amount.Value);

        if (!ValidateTransferRequest(request, result))
            return result; 

        var senderAccount = await _uow.BankAccounts.GetByIBANAsync(senderIBAN);

        if (!_bankAccountHelper.ValidateBankAccount(senderAccount, senderIBAN, result))
            return result;

        var recipientAccount = await _uow.BankAccounts.GetByIBANAsync(recipientIBAN);

        if (!_bankAccountHelper.ValidateBankAccount(recipientAccount, recipientIBAN, result))
            return result;

        var amountToTransfer = decimal.Round(request.BaseCashTransaction.Amount.Value, 2);
        var fees = decimal.Round(amountToTransfer * TransferFeePercentage, 2);
        var totalAmount = amountToTransfer + fees;

        if (!_bankAccountHelper.HasSufficientFunds(senderAccount, totalAmount, result))
            return result;

        // Execute transfer
        var accountOwner = _appUserAccessor.GetDisplayName();
        var transferDto = PrepareTransferDto(senderAccount, recipientAccount, amountToTransfer, fees);
        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, accountOwner, transferDto);

        // Apply domain logic to both accounts
        _bankAccountService.CreateCashTransaction(senderAccount, recipientAccount, cashTransaction, fees);

        // Mark both aggregates as modified for EF Core
        _uow.BankAccounts.Update(senderAccount);
        _uow.BankAccounts.Update(recipientAccount);

        // mark business result as completed on the object before saving so EF persists it in same transaction
        cashTransaction.UpdateStatus(CashTransactionStatus.Completed);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                  "Fund transfer completed successfully - TransactionId: {TransactionId}, " +
                  "From: {From}, To: {To}, Amount: {Amount}, Fees: {Fees}, Status: {Status}",
                  cashTransaction.Id,
                  senderIBAN,
                  recipientIBAN,
                  amountToTransfer,
                  fees,
                  cashTransaction.Status);
        }
        else
        {
            _logger.LogError(
                "Failed to persist fund transfer from {SenderIBAN} to {RecipientIBAN}. " +
                "Database transaction returned 0 rows affected",
                senderIBAN,
                recipientIBAN);

            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
        }

        return result;
    }

    #region Validation Methods

    /// <summary>
    /// Validates the transfer request (amount, IBANs presence)
    /// </summary>
    private static bool ValidateTransferRequest(MakeFundsTransferCommand request, ApiResult<Unit> result)
    {
        if (request?.BaseCashTransaction == null)
        {
            result.AddError(ErrorCode.BadRequest, "Invalid transfer request.");
            return false;
        }

        if (request.BaseCashTransaction.Amount.Value <= 0)
        {
            result.AddError(ErrorCode.BadRequest, "Transfer amount must be greater than zero.");
            return false;
        }

        var senderIBAN = request.From;
        var recipientIBAN = request.To;

        if (senderIBAN.Equals(recipientIBAN, StringComparison.OrdinalIgnoreCase))
        {
            result.AddError(ErrorCode.BadRequest, "Sender and recipient IBANs cannot be the same.");
            return false;
        }

        return true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Prepares transfer DTO with updated balances and recipient information
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when recipient has no owners</exception>
    private static TransferDto PrepareTransferDto(
        BankAccount? senderAccount,
        BankAccount? recipientAccount,
        decimal amountToTransfer,
        decimal fees)
    {
        ArgumentNullException.ThrowIfNull(senderAccount);
        ArgumentNullException.ThrowIfNull(recipientAccount);

        var recipientOwner = recipientAccount.BankAccountOwners.FirstOrDefault()?.Customer
            ?? throw new InvalidOperationException("Recipient account has no owners.");

        var updatedSenderBalance = decimal.Round(senderAccount.Balance - (amountToTransfer + fees), 2);
        var updatedRecipientBalance = decimal.Round(recipientAccount.Balance + amountToTransfer, 2);

        return new TransferDto(
            $"{recipientOwner.FirstName} {recipientOwner.LastName}",
            updatedSenderBalance,
            updatedRecipientBalance,
            fees
        );
    }

    #endregion
}