using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CreditCard.Responses;

namespace OnlineBanking.Application.Features.CreditCards.Queries;

public class GetCreditCardDetailsByIdRequest : IRequest<ApiResult<CreditCardDetailsResponse>>
{
  public Guid Id { get; set; }
}
