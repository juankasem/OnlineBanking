using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetAllCashTransactionsRequest : IRequest<ApiResult<CashTransactionListResponse>>
{

}
