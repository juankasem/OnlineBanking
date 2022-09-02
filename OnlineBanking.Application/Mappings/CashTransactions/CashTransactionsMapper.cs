using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models.CashTransaction;
using OnlineBanking.Application.Models.CashTransaction.Base;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Mappings.CashTransactions;

public class CashTransactionsMapper : ICashTransactionsMapper
{
    public MakeDepositCommand MapToMakeDepositCommand(CreateCashTransactionRequest request) =>
    new(CreateBaseCashTransactionDto(request), request.To);

    public MakeWithdrawalCommand MapToMakeWithdrawalCommand(CreateCashTransactionRequest request) =>
            new(CreateBaseCashTransactionDto(request), request.From);

    public MakeFundsTransferCommand MapToFundsTransferCommand(CreateCashTransactionRequest request) =>
        new(CreateBaseCashTransactionDto(request), request.From, request.To, request.Sender, request.Recipient);

    public CashTransactionResponse MapToResponseModel(CashTransaction ct, string iban)
    {
        var currency = CreateCurrency(ct.Currency);

        return new CashTransactionResponse(
                    ct.Id.ToString(),
                    ct.Type,
                    ct.InitiatedBy,
                    ct.From,
                    ct.To,
                    ct.Sender ?? null,
                    ct.Recipient ?? null,
                    ct.From != iban
                    ?
                    CreateMoney(ct.Amount, currency)
                    :
                    CreateMoney(-ct.Amount, currency),
                    CreateMoney(ct.Fees, currency),
                    ct.Description,
                    ct.PaymentType,
                    ct.TransactionDate,
                    ct.Status,
                    ct.To != iban
                    ?
                    CreateMoney(ct.SenderAvailableBalance, currency)
                    :
                    CreateMoney(ct.RecipientAvailableBalance, currency),
                    ct.CreditCardNo ?? null,
                    ct.DebitCardNo ?? null
        );
    }

    #region Private helper methods
    private BaseCashTransactionDto CreateBaseCashTransactionDto(CreateCashTransactionRequest request)
    {
        var ct = request.BaseCashTransaction;

        return new(ct.ReferenceNo, ct.Type,
                    ct.InitiatedBy, ct.Amount, ct.Fees,
                    ct.Description, ct.PaymentType, ct.TransactionDate,
                    ct.Status);
    }
   
    private Money CreateMoney(decimal amount, CurrencyDto currency) =>
        new(amount, currency);

    private CurrencyDto CreateCurrency(Currency currency) =>
     new (currency.Id, currency.Code, currency.Name, currency.Symbol);
    #endregion
}