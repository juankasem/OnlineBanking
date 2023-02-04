
using System.Collections.Immutable;

namespace OnlineBanking.Application.Models.CreditCard.Responses;

public class CreditCardListResponse
{
    public ImmutableList<CreditCardDto> CreditCards { get; set; }

    public int Count { get; set; }

    public CreditCardListResponse(ImmutableList<CreditCardDto> creditCards, int count)
    {
        CreditCards = creditCards;
        Count = count;
    }
}
