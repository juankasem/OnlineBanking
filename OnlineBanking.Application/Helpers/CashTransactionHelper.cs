using OnlineBanking.Application.Features.CashTransactions.Create.Deposit;
using OnlineBanking.Application.Features.CashTransactions.Create.Transfer;
using OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;

namespace OnlineBanking.Application.Helpers;

internal static class CashTransactionHelper
{
    public static CashTransaction CreateCashTransaction(MakeDepositCommand request, string recipient, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.Type, ct.InitiatedBy,
                                        GetInitiatorCode(ct.InitiatedBy),
                                        request.To, ct.Amount.Value, ct.Amount.CurrencyId,
                                        0, ct.Description, 0, updatedBalance,
                                        ct.PaymentType, DateTimeHelper.ConvertToDate(ct.TransactionDate),
                                        "Unknown", recipient);
    }

    public static CashTransaction CreateCashTransaction(MakeWithdrawalCommand request, string sender, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.Type, ct.InitiatedBy, request.From, GetInitiatorCode(ct.InitiatedBy),
                                      ct.Amount.Value, ct.Amount.CurrencyId, 0,
                                      ct.Description, updatedBalance, 0,
                                      ct.PaymentType, DateTimeHelper.ConvertToDate(ct.TransactionDate),
                                      sender, "Unknown");
    }

    public static CashTransaction CreateCashTransaction(MakeFundsTransferCommand request, string senderName, TransferDto transferDto)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.Type, ct.InitiatedBy,
                                    request.From, request.To, ct.Amount.Value, ct.Amount.CurrencyId,
                                    transferDto.Fees, ct.Description, transferDto.SenderBankAccountBalance, 
                                    transferDto.RecipientBankAccountBalance, ct.PaymentType, DateTimeHelper.ConvertToDate(ct.TransactionDate),
                                    senderName, transferDto.RecipientFullName);
    }

    private static string GetInitiatorCode(BankAssetType initiatedBy)
    {
        var code = initiatedBy == BankAssetType.ATM ? InitiatorCode.ATM :
                initiatedBy == BankAssetType.BankAccount ? InitiatorCode.BankAccount :
                initiatedBy == BankAssetType.POS ?
                InitiatorCode.POS : InitiatorCode.Unknown;

        return code;
    }
}

