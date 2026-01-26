using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;

namespace OnlineBanking.Application.Models.BankAccount;

public class BankAccountDto
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public AccountOwnerDto[] AccountOwners { get; set; }
    public BankAccountType Type { get; private set; }
    public BranchDto Branch { get; set; }
    public AccountBalanceDto AccountBalance { get; set; }
    public CurrencyDto Currency { get; set; }

    private BankAccountDto()
    {
    }

    public BankAccountDto(
        string accountNo, 
        string iban,
        AccountOwnerDto[] accountOwners, 
        BankAccountType type,
        BranchDto branch, 
        AccountBalanceDto accountBalance, 
        CurrencyDto currency)
    {
        AccountNo = accountNo;
        IBAN = iban;
        AccountOwners = accountOwners;
        Type = type;
        Branch = branch;
        AccountBalance = accountBalance;
        Currency = currency;
    }
}