using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;
using System.Collections.ObjectModel;

namespace OnlineBanking.Application.Models.BankAccount.Responses;

public class BankAccountResponse(string accountNo, string iban, BankAccountType type,
                            BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency,
                            ReadOnlyCollection<AccountOwnerDto> accountOwners,
                            ReadOnlyCollection<AccountTransactionDto> cashTransactions,
                            ReadOnlyCollection<AccountFastTransactionDto> fastTransactions,
                            ReadOnlyCollection<CreditCardDto> creditCards,
                            ReadOnlyCollection<DebitCardDto> debitCards)
{
    public string AccountNo { get; private set; } = accountNo;
    public string IBAN { get; private set; } = iban;
    public BankAccountType Type { get; private set; } = type;
    public BranchDto Branch { get; set; } = branch;
    public AccountBalanceDto AccountBalance { get; set; } = accountBalance;
    public CurrencyDto Currency { get; set; } = currency;
    public ReadOnlyCollection<AccountOwnerDto> AccountOwners { get; set; } = accountOwners;
    public ReadOnlyCollection<AccountTransactionDto> CashTransactions { get; set; } = cashTransactions;
    public ReadOnlyCollection<AccountFastTransactionDto> FastTransactions { get; set; } = fastTransactions;
    public ReadOnlyCollection<CreditCardDto> CreditCards { get; set; } = creditCards;
    public ReadOnlyCollection<DebitCardDto> DebitCards { get; set; } = debitCards;
}