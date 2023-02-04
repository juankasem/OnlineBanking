namespace OnlineBanking.Application.Models.CreditCard.Responses;

public class CreditCardDetailsResponse
{
    public CreditCardDto CreditCardDto { get; set; }

    public string CreditCardHolderName { get; set; }
    
    public string BillingAddress { get; set; }
}