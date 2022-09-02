using System.Collections.Generic;
using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;
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
    public List<AccountOwnerDto> AccountOwners { get; set; }
    public List<AccountCashTransactionDto> AccountCashTransactions { get; set; }
    public List<AccountFastTransactionDto> AccountFastTransactions { get; set; }
    public List<CreditCardDto> AccountCreditCards { get; set; }
    public List<DebitCardDto> AccountDebitCards { get; set; }

    public BankAccountDto(string accountNo, string iban, BankAccountType type,
                                BranchDto branch, AccountBalanceDto accountBalance, CurrencyDto currency,
                                List<AccountOwnerDto> accountOwners, List<AccountCashTransactionDto> accountCashTransactions,
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
        AccountCashTransactions = accountCashTransactions;
        AccountFastTransactions = accountFastTransactions;
        AccountCreditCards = accountCreditCards;
        AccountDebitCards = accountDebitCards;
    }
}