
using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.Currency;

namespace OnlineBanking.Application.Models.BankAccount;

public class BankAccountDto
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public string AccountOwner { get; set; }
    public BankAccountType Type { get; private set; }
    public BranchDto Branch { get; set; }
    public AccountBalanceDto AccountBalance { get; set; }
    public CurrencyDto Currency { get; set; }


    private BankAccountDto()
    {
    }

    public BankAccountDto(string accountNo, string iban, string accountOwner, BankAccountType type,
                          BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency)
    {
        AccountNo = accountNo;
        IBAN = iban;
        AccountOwner = accountOwner;
        Type = type;
        Branch = branch;
        AccountBalance = accountBalance;
        Currency = currency;
    }
}