namespace OnlineBanking.Application.Models.CreditCard.Base;

public class BaseCreditCardDto
{
    public string CreditCardNo { get; set; }
    public string CustomerNo { get; set; }
    public DateTime ValidTo { get; set; }
    public int SecurityCode { get; set; }
}