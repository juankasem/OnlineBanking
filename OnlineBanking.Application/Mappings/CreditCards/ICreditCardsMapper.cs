using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.CreditCard.Responses;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Mappings.CreditCards;

public interface ICreditCardsMapper
{
    public CreditCardDto MapToDto(CreditCard creditCard);
    public CreditCardDetailsResponse MapToResponseModel(CreditCard creditCard);

}
