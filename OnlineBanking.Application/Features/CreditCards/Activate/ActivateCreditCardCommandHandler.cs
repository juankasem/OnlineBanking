namespace OnlineBanking.Application.Features.CreditCards.Activate;

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
