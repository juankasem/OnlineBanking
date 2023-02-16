using System.Collections.Immutable;
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.FastTransactions.Queries;

public class GetFastTransactionsByIBANRequest : IRequest<ApiResult<ImmutableList<FastTransactionDto>>>
{
    public string IBAN { get; set; }
}
