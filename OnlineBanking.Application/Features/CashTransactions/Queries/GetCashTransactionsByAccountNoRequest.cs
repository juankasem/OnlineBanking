using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetCashTransactionsByAccountNoRequest : IRequest<ApiResult<CashTransactionListResponse>>
{
    public string AccountNo { get; set; }
}