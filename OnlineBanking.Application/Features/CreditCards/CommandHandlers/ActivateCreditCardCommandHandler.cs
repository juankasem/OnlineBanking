using OnlineBanking.Application.Features.CreditCards.Commands;

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
