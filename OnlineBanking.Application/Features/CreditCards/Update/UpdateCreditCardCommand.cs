namespace OnlineBanking.Application.Features.CreditCards.Update;

public class UpdateCreditCardCommand : IRequest<ApiResult<Unit>>
{
    public Guid CreditCardId { get; set; }
    public string CreditCardNo { get; set; }
    public string CustomerNo { get; set; }
    public DateTime ValidTo { get; set; }
    public int SecurityCode { get; set; }
    public Guid BankAccountId { get; set; }


    public UpdateCreditCardCommand(Guid creditCardId, string creditCardNo, string customerNo,
                                    DateTime validTo, int securityCode, Guid bankAccountId)
    {
        CreditCardId = creditCardId;
        CreditCardNo = creditCardNo;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
        BankAccountId = bankAccountId;
    }
}
