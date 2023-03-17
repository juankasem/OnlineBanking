using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CreditCard;

namespace OnlineBanking.Application.Features.CreditCards.Queries;

public class GetCustomerCreditCardsRequest : IRequest<ApiResult<IReadOnlyList<CreditCardDto>>>
{
    public string CustomerNo { get; set; }
}