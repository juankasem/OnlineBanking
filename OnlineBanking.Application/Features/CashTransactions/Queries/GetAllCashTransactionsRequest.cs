using MediatR;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetAllCashTransactionsRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
 public CashTransactionParams CashTransactionParams { get; set; }

}
