
using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Deposit;

public class MakeDepositCommand : IRequest<ApiResult<Unit>>
{
    public BaseCashTransactionDto BaseCashTransaction { get; set; }
    public string To { get; set; }

    public MakeDepositCommand(BaseCashTransactionDto baseCashTransaction, string to)
    {
        BaseCashTransaction = baseCashTransaction;
        To = to;
    }
}