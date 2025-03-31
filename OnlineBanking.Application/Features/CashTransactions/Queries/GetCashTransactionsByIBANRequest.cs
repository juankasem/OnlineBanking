using MediatR;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;


namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetCashTransactionsByAccountNoOrIBANRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
    public string IBAN { get; set; }
    public CashTransactionParams CashTransactionParams { get; set; }
}