using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.BankAccount.Responses;

public class BankAccountResponse
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public BankAccountType Type { get; private set; }
    public BranchDto Branch { get; set; }
    public AccountBalanceDto AccountBalance { get; set; }
    public CurrencyDto Currency { get; set; }
    public List<AccountOwnerDto> AccountOwners { get; set; } = new List<AccountOwnerDto>();
    public List<AccountTransactionDto> AccountTransactions { get; set; } = new List<AccountTransactionDto>();
    public List<AccountFastTransactionDto> AccountFastTransactions { get; set; } = new List<AccountFastTransactionDto>();
    public List<CreditCardDto> AccountCreditCards { get; set; } = new List<CreditCardDto>();
    public List<DebitCardDto> AccountDebitCards { get; set; } = new List<DebitCardDto>();

    public BankAccountResponse(string accountNo, string iban, BankAccountType type,
                                BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency,
                                List<AccountOwnerDto> accountOwners, List<AccountTransactionDto> accountTransactions,
                                List<AccountFastTransactionDto> accountFastTransactions,
                                List<CreditCardDto> accountCreditCards, List<DebitCardDto> accountDebitCards)
    {
        AccountNo = accountNo;
        IBAN = iban;
        Type = type;
        Branch = branch;
        AccountBalance = accountBalance;
        Currency = currency;
        AccountOwners = accountOwners;
        AccountTransactions = accountTransactions;
        AccountFastTransactions = accountFastTransactions;
        AccountCreditCards = accountCreditCards;
        AccountDebitCards = accountDebitCards;
    }
}