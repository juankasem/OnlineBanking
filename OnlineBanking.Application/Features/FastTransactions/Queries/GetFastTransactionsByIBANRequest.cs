using System.Collections.Immutable;
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.Application.Features.FastTransactions.Queries;

public class GetFastTransactionsByIBANRequest : IRequest<ApiResult<ImmutableList<FastTransactionResponse>>>
{
    public string IBAN { get; set; }
}
