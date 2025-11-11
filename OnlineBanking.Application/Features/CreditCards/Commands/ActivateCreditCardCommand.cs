namespace OnlineBanking.Application.Features.CreditCards.Commands;

public class ActivateCreditCardCommand : IRequest<ApiResult<Unit>>
{
    public string CreditCardNo { get; set; }
}
