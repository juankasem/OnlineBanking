using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetByAccountNoOrIBAN;

public class GetCashTransactionsByAccountNoOrIBANRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
    public string AccountNoOrIBAN { get; set; }
    public CashTransactionParams CashTransactionParams { get; set; }
}