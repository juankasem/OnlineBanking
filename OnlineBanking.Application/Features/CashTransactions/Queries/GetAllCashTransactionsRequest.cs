using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetAllCashTransactionsRequest : IRequest<ApiResult<PagedList<CashTransactionResponse>>>
{
 public CashTransactionParams CashTransactionParams { get; set; }

}
