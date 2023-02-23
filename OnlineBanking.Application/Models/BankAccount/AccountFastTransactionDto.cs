namespace OnlineBanking.Application.Models.BankAccount;

public class AccountFastTransactionDto
{
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public decimal Amount { get; set; }
}
