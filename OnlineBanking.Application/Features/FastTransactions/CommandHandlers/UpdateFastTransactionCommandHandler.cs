
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.FastTransactions.CommandHandlers;

public class UpdateFastTransactionCommandHandler : IRequestHandler<UpdateFastTransactionCommand, ApiResult<Unit>>
{

    public UpdateFastTransactionCommandHandler()
    {

    }

    public Task<ApiResult<Unit>> Handle(UpdateFastTransactionCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
