using OnlineBanking.Application.Models.CreditCard.Responses;

namespace OnlineBanking.Application.Features.CreditCards.GetById;

public class GetCreditCardByIdRequest : IRequest<ApiResult<CreditCardDetailsResponse>>
{
    public Guid Id { get; set; }
}
