using MediatR;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.Application.Features.FastTransactions.Queries;

public class GetFastTransactionsByIBANRequest : IRequest<ApiResult<PagedList<FastTransactionResponse>>>
{
    public string IBAN { get; set; }

    public FastTransactionParams FastTransactionParams { get; set; }
}
