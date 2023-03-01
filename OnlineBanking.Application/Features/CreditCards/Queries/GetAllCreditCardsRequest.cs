using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.CreditCards.Queries;

public class GetAllCreditCardsRequest : IRequest<ApiResult<PagedList<CreditCardDto>>>
{
    public CreditCardParams  CreditCardParams { get; set; }
}