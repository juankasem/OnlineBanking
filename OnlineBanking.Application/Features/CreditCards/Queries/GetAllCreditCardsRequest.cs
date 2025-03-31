using MediatR;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CreditCard;

namespace OnlineBanking.Application.Features.CreditCards.Queries;

public class GetAllCreditCardsRequest : IRequest<ApiResult<PagedList<CreditCardDto>>>
{
    public CreditCardParams CreditCardParams { get; set; }
}