
using OnlineBanking.Application.Common.Helpers;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models.CashTransaction;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;

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
                                        null, recipient);
    }

    public static CashTransaction CreateCashTransaction(MakeWithdrawalCommand request, string sender, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.Type, ct.InitiatedBy, request.From, GetInitiatorCode(ct.InitiatedBy),
                                      ct.Amount.Value, ct.Amount.CurrencyId, 0,
                                      ct.Description, updatedBalance, 0,
                                      ct.PaymentType, DateTimeHelper.ConvertToDate(ct.TransactionDate),
                                      sender, null);
    }

    public static CashTransaction CreateCashTransaction(MakeFundsTransferCommand request, TransferDto transferDto)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.Type, ct.InitiatedBy,
                                    request.From, request.To, ct.Amount.Value, ct.Amount.CurrencyId,
                                    transferDto.Fees, ct.Description, transferDto.SenderBalance, transferDto.RecipientBalance,
                                    ct.PaymentType, DateTimeHelper.ConvertToDate(ct.TransactionDate),
                                    transferDto.SenderFullName, transferDto.RecipientFullName);
    }

    private static string GetInitiatorCode(BankAssetType initiatedBy)
    {
        return initiatedBy == BankAssetType.ATM ? InitiatorCode.ATM :
                                    initiatedBy == BankAssetType.Branch ? InitiatorCode.Branch :
                                    initiatedBy == BankAssetType.POS ?
                                    InitiatorCode.POS : InitiatorCode.Unknown;
    }
}

