using MediatR;
using OnlineBanking.Application.Features.CreditCards.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.CreditCards.CommandHandlers;

public class DeactivateCreditCardCommandHandler : IRequestHandler<DeactivateCreditCardCommand, ApiResult<Unit>>
{

    public DeactivateCreditCardCommandHandler()
    {
        
    }

    public Task<ApiResult<Unit>> Handle(DeactivateCreditCardCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
