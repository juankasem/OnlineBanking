using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.FastTransactions.QueryHandlers;

public class GetFastTransactionsByIBANRequestHandler : IRequestHandler<GetFastTransactionsByIBANRequest, ApiResult<ImmutableList<FastTransactionDto>>>
    {
    public GetFastTransactionsByIBANRequestHandler()
    {

    }


    public Task<ApiResult<ImmutableList<FastTransactionDto>>> Handle(GetFastTransactionsByIBANRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
