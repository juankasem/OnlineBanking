

using MediatR;
using OnlineBanking.Application.Features.CreditCards.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.CreditCards.CommandHandlers;

public class ActivateCreditCardCommandHandler : IRequestHandler<ActivateCreditCardCommand, ApiResult<Unit>>
{
    public ActivateCreditCardCommandHandler()
    {
        
    }

    public Task<ApiResult<Unit>> Handle(ActivateCreditCardCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
