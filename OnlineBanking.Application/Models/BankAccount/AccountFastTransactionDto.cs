using OnlineBanking.Application.Models.CashTransaction;

namespace OnlineBanking.Application.Models.BankAccount;

public class AccountFastTransactionDto
{
    public string IBAN { get; set; }
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public string RecipientBankName { get; set; }
    public Money Amount { get; set; }


    public AccountFastTransactionDto(string iBAN, string recipientIBAN, string recipientName, string recipientBankName, Money amount)
    {
        IBAN = iBAN;
        RecipientIBAN = recipientIBAN;
        RecipientName = recipientName;
        RecipientBankName = recipientBankName;
        Amount = amount;
    }
}
