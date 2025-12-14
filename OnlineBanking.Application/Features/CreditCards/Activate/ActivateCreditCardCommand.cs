namespace OnlineBanking.Application.Features.CreditCards.Activate;

public class ActivateCreditCardCommand : IRequest<ApiResult<Unit>>
{
    public string CreditCardNo { get; set; }
}
