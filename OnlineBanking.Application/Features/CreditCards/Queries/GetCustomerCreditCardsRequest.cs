using MediatR;
using OnlineBanking.Application.Models.CreditCard.Responses;

namespace OnlineBanking.Application.Features.CreditCards.Queries;

public class GetCustomerCreditCardsRequest : IRequest<CreditCardListResponse>
{
    public string CustomerNo { get; set; }
}