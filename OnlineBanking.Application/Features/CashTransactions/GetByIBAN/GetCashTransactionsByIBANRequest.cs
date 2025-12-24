
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetByIBAN;

public class GetCashTransactionsByAccountNoOrIBANRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
    public string IBAN { get; set; }
    public CashTransactionParams CashTransactionParams { get; set; }
}