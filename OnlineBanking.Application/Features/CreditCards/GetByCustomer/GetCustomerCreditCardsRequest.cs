using OnlineBanking.Application.Models.CreditCard;

namespace OnlineBanking.Application.Features.CreditCards.GetByCustomer;

public class GetCustomerCreditCardsRequest : IRequest<ApiResult<IReadOnlyList<CreditCardDto>>>
{
    public string CustomerNo { get; set; }
}