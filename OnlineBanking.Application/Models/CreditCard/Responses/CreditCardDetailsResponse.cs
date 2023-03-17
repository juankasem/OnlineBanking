namespace OnlineBanking.Application.Models.CreditCard.Responses;

public class CreditCardDetailsResponse
{
    public string CreditCardHolderName { get; set; }
    public string CreditCardNo { get; set; }
    public string CustomerNo { get; set; }
    public DateTime ValidTo { get; set; }
    public int SecurityCode { get; set; }
    public decimal AvailableBalance { get; set; }
    public bool IsActive { get; set; }

    public CreditCardDetailsResponse(string creditCardHolderName, string creditCardNo,
                                    string customerNo, DateTime validTo, int securityCode,
                                    decimal availableBalance, bool isActive)
    {
        CreditCardHolderName = creditCardHolderName;
        CreditCardNo = creditCardNo;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
        AvailableBalance = availableBalance;
        IsActive = isActive;
    }
}