using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;
using OnlineBanking.Core.Domain.Enums;
using System.Collections.ObjectModel;

namespace OnlineBanking.Application.Models.BankAccount.Responses;

public class BankAccountResponse
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public BankAccountType Type { get; private set; }
    public BranchDto Branch { get; set; }
    public AccountBalanceDto AccountBalance { get; set; }
    public CurrencyDto Currency { get; set; }
    public ReadOnlyCollection<AccountOwnerDto> AccountOwners { get; set; } 
    public ReadOnlyCollection<AccountTransactionDto> CashTransactions { get; set; }
    public ReadOnlyCollection<AccountFastTransactionDto> FastTransactions { get; set; }
    public ReadOnlyCollection<CreditCardDto> CreditCards { get; set; } 
    public ReadOnlyCollection<DebitCardDto> DebitCards { get; set; }

    public BankAccountResponse(string accountNo, string iban, BankAccountType type,
                                BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency,
                                ReadOnlyCollection<AccountOwnerDto> accountOwners, 
                                ReadOnlyCollection<AccountTransactionDto> cashTransactions,
                                ReadOnlyCollection<AccountFastTransactionDto> fastTransactions,
                                ReadOnlyCollection<CreditCardDto> creditCards, 
                                ReadOnlyCollection<DebitCardDto> debitCards)
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