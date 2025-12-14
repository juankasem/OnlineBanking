namespace OnlineBanking.Application.Features.CreditCards.Deactivate;

public class DeactivateCreditCardCommand : IRequest<ApiResult<Unit>>
{
    public string CreditCardNo { get; set; }
}