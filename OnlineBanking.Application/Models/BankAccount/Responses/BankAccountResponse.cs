using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;

namespace OnlineBanking.Application.Models.BankAccount.Responses;

public class BankAccountResponse(
    string accountNo, 
    string iban, 
    BankAccountType type,
    BranchDto branch, 
    AccountBalanceDto accountBalance, 
    CurrencyDto currency,
    AccountOwnerDto[] accountOwners,
    AccountTransactionDto[] cashTransactions,
    AccountFastTransactionDto[] fastTransactions,
    CreditCardDto[] creditCards,
    DebitCardDto[] debitCards)
{
    public string AccountNo { get; private set; } = accountNo;
    public string IBAN { get; private set; } = iban;
    public BankAccountType Type { get; private set; } = type;
    public BranchDto Branch { get; private set; } = branch;
    public AccountBalanceDto AccountBalance { get; private set; } = accountBalance;
    public CurrencyDto Currency { get; private set; } = currency;
    public AccountOwnerDto[] AccountOwners { get; private set; } = accountOwners;
    public AccountTransactionDto[] CashTransactions { get; private set; } = cashTransactions;
    public AccountFastTransactionDto[] FastTransactions { get; private set; } = fastTransactions;
    public CreditCardDto[] CreditCards { get; private set; } = creditCards;
    public DebitCardDto[] DebitCards { get; private set; } = debitCards;
}