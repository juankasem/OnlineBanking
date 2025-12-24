
using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;

public class MakeWithdrawalCommand : IRequest<ApiResult<Unit>>
{
    public BaseCashTransactionDto BaseCashTransaction { get; set; }
    public string From { get; set; }

    public MakeWithdrawalCommand(BaseCashTransactionDto baseCashTransaction, string from)
    {
        BaseCashTransaction = baseCashTransaction;
        From = from;
    }
}