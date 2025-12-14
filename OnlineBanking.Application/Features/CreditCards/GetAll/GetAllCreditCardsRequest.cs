using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models.CreditCard;

namespace OnlineBanking.Application.Features.CreditCards.GetAll;

public class GetAllCreditCardsRequest : IRequest<ApiResult<PagedList<CreditCardDto>>>
{
    public CreditCardParams CreditCardParams { get; set; }
}