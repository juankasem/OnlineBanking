using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models.CashTransaction.Responses;


namespace OnlineBanking.Application.Features.CashTransactions.GetByAccountNo;

public class GetAccountTransactionsRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
    public string AccountNoOrIBAN { get; set; }
    public CashTransactionParams CashTransactionParams { get; set; }
}