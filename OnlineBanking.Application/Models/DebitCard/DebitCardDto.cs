using OnlineBanking.Application.Models.DebitCard.Base;

namespace OnlineBanking.Application.Models.DebitCard;

public class DebitCardDto : BaseDebitCardDto
{
    public string DebitCardHolder { get; set; }

    public DebitCardDto(
        string debitCardHolder,
        string debitCardNo,
        string customerNo,
        DateTime validTo,
        int securityCode)
    {
        DebitCardHolder = debitCardHolder;
        DebitCardNo = debitCardNo;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
    }
}