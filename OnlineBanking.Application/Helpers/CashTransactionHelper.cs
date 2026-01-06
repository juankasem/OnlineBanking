namespace OnlineBanking.Application.Helpers;

/// <summary>
/// Helper class for creating cash transactions from command requests.
/// Provides factory methods to construct CashTransaction aggregates with proper data mapping.
/// </summary>
internal static class CashTransactionHelper
{
    private const string UnknownParticipant = "Unknown";

    /// <summary>
    /// Creates a deposit cash transaction from a deposit command.
    /// </summary>
    /// <param name="request">The deposit command containing transaction details</param>
    /// <param name="recipient">The name of the recipient (account owner)</param>
    /// <param name="updatedBalance">The updated balance after the deposit</param>
    /// <returns>A new CashTransaction aggregate for the deposit</returns>
    public static CashTransaction CreateCashTransaction(
        MakeDepositCommand request, 
        string recipient, 
        decimal updatedBalance)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);

        var baseTransaction = request.BaseCashTransaction;

        return CashTransaction.Create(
            type: baseTransaction.Type,
            initiatedBy: baseTransaction.InitiatedBy,
            from: GetInitiatorCode(baseTransaction.InitiatedBy),
            to: request.To,
            amount: baseTransaction.Amount.Value,
            currencyId: baseTransaction.Amount.CurrencyId,
            fees: 0,
            description: baseTransaction.Description,
            senderAvailableBalance: 0,
            recipientAvailableBalance: updatedBalance,
            paymentType: baseTransaction.PaymentType,
            transactionDate: baseTransaction.TransactionDate,
            sender: UnknownParticipant,
            recipient: recipient);
    }

    /// <summary>
    /// Creates a withdrawal cash transaction from a withdrawal command.
    /// </summary>
    /// <param name="request">The withdrawal command containing transaction details</param>
    /// <param name="sender">The name of the sender (account owner)</param>
    /// <param name="updatedBalance">The updated balance after the withdrawal</param>
    /// <returns>A new CashTransaction aggregate for the withdrawal</returns>
    public static CashTransaction CreateCashTransaction(
        MakeWithdrawalCommand request,
        string sender,
        decimal updatedBalance)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(sender);

        var baseTransaction = request.BaseCashTransaction;

        return CashTransaction.Create(
               type: baseTransaction.Type,
               initiatedBy: baseTransaction.InitiatedBy,
               from: request.From,
               to: GetInitiatorCode(baseTransaction.InitiatedBy),
               amount: baseTransaction.Amount.Value,
               currencyId: baseTransaction.Amount.CurrencyId,
               fees: 0,
               description: baseTransaction.Description,
               senderAvailableBalance: updatedBalance,
               recipientAvailableBalance: 0,
               paymentType: baseTransaction.PaymentType,
               transactionDate: baseTransaction.TransactionDate,
               sender: sender,
               recipient: UnknownParticipant);
    }

    /// <summary>
    /// Creates a transfer cash transaction from a transfer command.
    /// </summary>
    /// <param name="request">The transfer command containing transaction details</param>
    /// <param name="senderName">The name of the sender</param>
    /// <param name="transferDto">The transfer details including balances and fees</param>
    /// <returns>A new CashTransaction aggregate for the transfer</returns>
    public static CashTransaction CreateCashTransaction(
        MakeFundsTransferCommand request,
        string senderName,
        TransferDto transferDto)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(senderName);
        ArgumentNullException.ThrowIfNull(transferDto);

        var baseTransaction = request.BaseCashTransaction;

        return CashTransaction.Create(
            type: baseTransaction.Type,
            initiatedBy: baseTransaction.InitiatedBy,
            from: request.From,
            to: request.To,
            amount: baseTransaction.Amount.Value,
            currencyId: baseTransaction.Amount.CurrencyId,
            fees: transferDto.Fees,
            description: baseTransaction.Description,
            senderAvailableBalance: transferDto.SenderBankAccountBalance,
            recipientAvailableBalance: transferDto.RecipientBankAccountBalance,
            paymentType: baseTransaction.PaymentType,
            transactionDate: baseTransaction.TransactionDate,
            sender: senderName,
            recipient: transferDto.RecipientFullName);
    }

    /// <summary>
    /// Maps a BankAssetType enum to its corresponding initiator code constant.
    /// Used to identify the source channel of the transaction (ATM, BankAccount, POS, etc).
    /// </summary>
    /// <param name="initiatedBy">The bank asset type that initiated the transaction</param>
    /// <returns>The corresponding initiator code constant</returns>
    private static string GetInitiatorCode(BankAssetType initiatedBy) =>
        initiatedBy switch
        {
            BankAssetType.ATM => InitiatorCode.ATM,
            BankAssetType.BankAccount => InitiatorCode.BankAccount,
            BankAssetType.POS => InitiatorCode.POS,
            _ => InitiatorCode.Unknown
        };
}

