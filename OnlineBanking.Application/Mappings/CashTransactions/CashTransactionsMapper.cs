using OnlineBanking.Application.Models.CashTransaction.Base;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Application.Models.Currency;

namespace OnlineBanking.Application.Mappings.CashTransactions;

/// <summary>
/// Mapper for cash transaction requests and responses.
/// Maps between request DTOs, command objects, and response models.
/// </summary>
public class CashTransactionsMapper : ICashTransactionsMapper
{
    /// <summary>
    /// Maps a cash transaction request to a deposit command.
    /// </summary>
    public MakeDepositCommand MapToMakeDepositCommand(CreateCashTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var baseCashTransaction = CreateBaseCashTransaction(request);
        return new(baseCashTransaction, request.To);
    }

    /// <summary>
    /// Maps a cash transaction request to a withdrawal command.
    /// </summary>
    public MakeWithdrawalCommand MapToMakeWithdrawalCommand(CreateCashTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var baseCashTransaction = CreateBaseCashTransaction(request);
        return new(baseCashTransaction, request.From);
    }

    /// <summary>
    /// Maps a cash transaction request to a funds transfer command.
    /// </summary>
    public MakeFundsTransferCommand MapToFundsTransferCommand(CreateCashTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var baseCashTransaction = CreateBaseCashTransaction(request);
        return new(baseCashTransaction, request.From, request.To, request.Sender, request.Recipient);
    }

    /// <summary>
    /// Maps a cash transaction domain entity to a response model.
    /// Formats the response based on whether the provided IBAN is sender or recipient.
    /// </summary>
    public CashTransactionResponse MapToResponseModel(CashTransaction ct, string iban)
    {
        ArgumentNullException.ThrowIfNull(ct);
        ArgumentException.ThrowIfNullOrWhiteSpace(iban);

        var currency = CreateCurrency(ct.Currency);
        var isSender = ct.From == iban;

        return new CashTransactionResponse(
            ct.Id.ToString(),
            Enum.GetName(ct.Type),
            Enum.GetName(ct.InitiatedBy),
            ct.From,
            ct.To,
            ct.Sender,
            ct.Recipient,
            CreateTransactionAmount(ct.Amount, currency, isSender),
            CreateMoney(ct.Fees, currency),
            ct.Description,
            Enum.GetName(ct.PaymentType),
            ct.TransactionDate.ToString("O"),
            Enum.GetName(ct.Status),
            CreateAccountBalance(ct.SenderAvailableBalance, ct.RecipientAvailableBalance, currency, isSender));
    }

    #region Private helper methods
    /// <summary>
    /// Creates a BaseCashTransaction from the request data.
    /// Uses named parameters to ensure all required properties are set correctly.
    /// </summary>
    private static BaseCashTransaction CreateBaseCashTransaction(CreateCashTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.BaseCashTransaction);

        var baseCashTransaction = request.BaseCashTransaction;

        return new BaseCashTransaction(
            iBAN: baseCashTransaction.IBAN,
            type: baseCashTransaction.Type,
            initiatedBy: baseCashTransaction.InitiatedBy,
            amount: baseCashTransaction.Amount,
            fees: baseCashTransaction.Fees,
            description: baseCashTransaction.Description,
            paymentType: baseCashTransaction.PaymentType,
            transactionDate: baseCashTransaction.TransactionDate);
    }

    /// <summary>
    /// Determines transaction amount sign based on sender/recipient perspective.
    /// Returns negative amount for withdrawals (from perspective of account owner),
    /// positive amount for deposits.
    /// </summary>
    private static Money CreateTransactionAmount(decimal amount, CurrencyDto currency, bool isSender)
    {
        var adjustedAmount = isSender ? -amount : amount;
        return CreateMoney(adjustedAmount, currency);
    }

    /// <summary>
    /// Selects appropriate account balance based on sender/recipient perspective.
    /// </summary>
    private static Money CreateAccountBalance(
        decimal senderBalance,
        decimal recipientBalance,
        CurrencyDto currency,
        bool isSender)
    {
        var balance = isSender ? senderBalance : recipientBalance;
        return CreateMoney(balance, currency);
    }

    /// <summary>
    /// Creates a Money model from decimal amount and currency.
    /// </summary>
    private static Money CreateMoney(decimal amount, CurrencyDto currency) =>
        new(amount, currency);

    /// <summary>
    /// Creates a CurrencyDto from domain Currency entity.
    /// </summary>
    private static CurrencyDto CreateCurrency(Currency currency) =>
     new(currency.Id, currency.Code, currency.Name, currency.Symbol);
    #endregion
}