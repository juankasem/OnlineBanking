
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetAll;

public class GetAllCashTransactionsRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
    public CashTransactionParams CashTransactionParams { get; set; }

}
