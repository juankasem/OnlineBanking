using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.Application.Features.FastTransactions.QueryHandlers;

public class GetFastTransactionsByIBANRequestHandler : IRequestHandler<GetFastTransactionsByIBANRequest, ApiResult<ImmutableList<FastTransactionResponse>>>
    {
    public GetFastTransactionsByIBANRequestHandler()
    {

    }


    public Task<ApiResult<ImmutableList<FastTransactionResponse>>> Handle(GetFastTransactionsByIBANRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
