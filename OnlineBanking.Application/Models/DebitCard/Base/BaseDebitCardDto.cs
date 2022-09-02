using System;

namespace OnlineBanking.Application.Models.DebitCard.Base;

public class BaseDebitCardDto
{
    public string DebitCardNo { get; set; }
    public DateTime ValidTo { get; set; }
    public string CustomerNo { get; set; }
    public int SecurityCode { get; set; }
}