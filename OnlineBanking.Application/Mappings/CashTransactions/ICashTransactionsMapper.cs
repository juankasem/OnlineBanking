using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Mappings.CashTransactions;

public interface ICashTransactionsMapper
{
    CashTransactionResponse MapToResponseModel(CashTransaction cashTransaction, string iban);
    MakeDepositCommand MapToMakeDepositCommand(CreateCashTransactionRequest request);
    MakeFundsTransferCommand MapToFundsTransferCommand(CreateCashTransactionRequest request);
    MakeWithdrawalCommand MapToMakeWithdrawalCommand(CreateCashTransactionRequest request);
}