
using MediatR;
using OnlineBanking.Application.Models.CreditCard.Responses;

namespace OnlineBanking.Application.Features.CreditCards.Queries;

public class GetCreditCardDetailsByIdRequest : IRequest<CreditCardDetailsResponse>
{
  public string Id { get; set; }
}
