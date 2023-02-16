
namespace OnlineBanking.Application.Models.FastTransaction.Base;
public class BaseFastTransactionDto
{
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public decimal Amount { get; set; }
}