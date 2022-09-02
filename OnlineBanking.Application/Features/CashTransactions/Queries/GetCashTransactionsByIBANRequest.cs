using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.Queries;

public class GetCashTransactionsByIBANRequest : IRequest<ApiResult<CashTransactionListResponse>>
{
    public string IBAN { get; set; }
}