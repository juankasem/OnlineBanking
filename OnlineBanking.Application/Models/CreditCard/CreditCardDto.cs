using OnlineBanking.Application.Models.CreditCard.Base;

namespace OnlineBanking.Application.Models.CreditCard;

public class CreditCardDto : BaseCreditCardDto
{
    public string CreditCardHolder { get; set; }

    public CreditCardDto(
        string creditCardHolder, 
        string creditCardNo,
        string customerNo, 
        DateTime validTo, 
        int securityCode)
    {
        CreditCardHolder = creditCardHolder;
        CreditCardNo = creditCardNo;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
    }
}