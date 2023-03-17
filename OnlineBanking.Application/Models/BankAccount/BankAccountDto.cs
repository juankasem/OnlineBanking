using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.BankAccount;

public class BankAccountDto
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public BankAccountType Type { get; private set; }
    public BranchDto Branch { get; set; }
    public AccountBalanceDto AccountBalance { get; set; }
    public CurrencyDto Currency { get; set; }

    public BankAccountDto(string accountNo, string iban, BankAccountType type,
                        BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency)
    {
        AccountNo = accountNo;
        IBAN = iban;
        Type = type;
        Branch = branch;
        AccountBalance = accountBalance;
        Currency = currency;
    }
}