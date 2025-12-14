namespace OnlineBanking.Application.Features.CreditCards.Deactivate;

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
