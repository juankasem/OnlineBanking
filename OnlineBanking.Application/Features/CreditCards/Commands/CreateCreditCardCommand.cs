namespace OnlineBanking.Application.Features.CreditCards.Commands;

public class CreateCreditCardCommand : IRequest<ApiResult<Unit>>
{
    public string CreditCardNo { get; set; }
    public string CustomerNo { get; set; }
    public DateTime ValidTo { get; set; }
    public int SecurityCode { get; set; }
    public Guid BankAccountId { get; set; }

    public CreateCreditCardCommand(string creditCardNo, string customerNo,
                                    DateTime validTo, int securityCode, Guid bankAccountId)
    {
        CreditCardNo = creditCardNo;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
        BankAccountId = bankAccountId;
    }
}
