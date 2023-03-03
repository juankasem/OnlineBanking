namespace OnlineBanking.Application.Models.CreditCard.Requests;

public class UpdateCreditCardRequest
{
    public Guid CreditCardId { get; set; }
    public string CreditCardNo { get; set; }
    public string CustomerNo { get; set; }
    public DateTime ValidTo { get; set; }
    public int SecurityCode { get; set; }
    public Guid BankAccountId { get; set; }
}