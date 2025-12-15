
using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Transfer;

public class MakeFundsTransferCommand : IRequest<ApiResult<Unit>>
{
    public BaseCashTransactionDto BaseCashTransaction { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Sender { get; set; }
    public string Recipient { get; set; }

    public MakeFundsTransferCommand(BaseCashTransactionDto baseCashTransaction,
                                    string from, string to,
                                    string sender, string recipient)
    {
        BaseCashTransaction = baseCashTransaction;
        From = from;
        To = to;
        Sender = sender;
        Recipient = recipient;
    }
}