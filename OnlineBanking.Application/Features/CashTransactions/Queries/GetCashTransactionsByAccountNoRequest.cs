using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetAccountTransactionsRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
    public string AccountNoOrIBAN { get; set; }
    public CashTransactionParams CashTransactionParams { get; set; }
}