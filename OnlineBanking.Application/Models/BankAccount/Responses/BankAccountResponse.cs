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
    public IReadOnlyList<AccountOwnerDto> AccountOwners { get; set; } 
    public IReadOnlyList<AccountTransactionDto> CashTransactions { get; set; }
    public IReadOnlyList<AccountFastTransactionDto> FastTransactions { get; set; }
    public IReadOnlyList<CreditCardDto> CreditCards { get; set; } 
    public IReadOnlyList<DebitCardDto> DebitCards { get; set; }

    public BankAccountResponse(string accountNo, string iban, BankAccountType type,
                                BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency,
                                IReadOnlyList<AccountOwnerDto> accountOwners, 
                                IReadOnlyList<AccountTransactionDto> cashTransactions,
                                IReadOnlyList<AccountFastTransactionDto> fastTransactions,
                                IReadOnlyList<CreditCardDto> creditCards, 
                                IReadOnlyList<DebitCardDto> debitCards)
    {
        AccountNo = accountNo;
        IBAN = iban;
        Type = type;
        Branch = branch;
        AccountBalance = accountBalance;
        Currency = currency;
        AccountOwners = accountOwners;
        CashTransactions = cashTransactions;
        FastTransactions = fastTransactions;
        CreditCards = creditCards;
        DebitCards = debitCards;
    }
}