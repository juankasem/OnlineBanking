using System.Collections.Generic;
using System.Linq;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CashTransaction;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Mappings.BankAccounts;

public class BankAccountMapper : IBankAccountMapper
{
    
    public BankAccountDto MapToDtoModel(BankAccount bankAccount)
    {
        var currency = MapToAccountCurrency(bankAccount.Currency);

        return new(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                        MapToAccountBranch(bankAccount.Branch),
                                        MapToAccountBalance(bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                                            bankAccount.MinimumAllowedBalance, bankAccount.Debt),
                                        currency,
                                        MapToAccountOwners(bankAccount.BankAccountOwners.ToList()),
                                        MapToAccountCashTransactions(bankAccount.AccountTransactions.ToList(), currency),
                                        MapToAccountFastTransactions(bankAccount.FastTransactions.ToList(), currency),
                                        MapToAccountCreditCards(bankAccount.CreditCards.ToList(), currency),
                                        MapToAccountDebitCards(bankAccount.DebitCards.ToList(), currency));
    }

    public BankAccountResponse MapToResponseModel(BankAccount bankAccount)
    {
        var currency = MapToAccountCurrency(bankAccount.Currency);

        return new(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                        MapToAccountBranch(bankAccount.Branch),
                                        MapToAccountBalance(bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                                            bankAccount.MinimumAllowedBalance, bankAccount.Debt),
                                        currency,
                                        MapToAccountOwners(bankAccount.BankAccountOwners.ToList()),
                                        MapToAccountCashTransactions(bankAccount.AccountTransactions.ToList(), currency),
                                        MapToAccountFastTransactions(bankAccount.FastTransactions.ToList(), currency),
                                        MapToAccountCreditCards(bankAccount.CreditCards.ToList(), currency),
                                        MapToAccountDebitCards(bankAccount.DebitCards.ToList(), currency));
    }

    #region Private helper methods
    private CurrencyDto MapToAccountCurrency(Currency currency) =>
        new(currency.Id, currency.Code, currency.Name, currency.Symbol);
    
    private BranchDto MapToAccountBranch(Branch branch) =>
        new(branch.Id, branch.Name);

    private AccountBalanceDto MapToAccountBalance(decimal balance, decimal allowedBalanceToUse,
                                                    decimal minimumAllowedBalance, decimal debt ) =>
        new (balance, allowedBalanceToUse, minimumAllowedBalance, debt);

    private List<AccountOwnerDto> MapToAccountOwners(List<CustomerBankAccount> customerBankAccounts)
    {
        var bankAccountOwners = new List<AccountOwnerDto>();

        foreach (var customerBankAccount in customerBankAccounts)
        {
            var bankAccountOwner = new AccountOwnerDto(customerBankAccount.Customer.Id, CreateFullName(customerBankAccount.Customer));

            bankAccountOwners.Add(bankAccountOwner);
        }
        return bankAccountOwners;
    }

    private List<AccountCashTransactionDto> MapToAccountCashTransactions(List<AccountTransaction> cashTransactions, CurrencyDto currency)
    {
        var accountTransactions = new List<AccountCashTransactionDto>();

        foreach (var cashTransaction in cashTransactions)
        {
            var ct = cashTransaction.Transaction;
            var accountTransaction = new AccountCashTransactionDto(ct.Type, ct.InitiatedBy,
                                                                CreateMoney(ct.Amount, currency), CreateMoney(ct.Fees, currency),
                                                                ct.Description, ct.PaymentType, ct.TransactionDate, ct.Status,
                                                                ct.From, ct.To, ct.Sender, ct.Recipient);

            accountTransactions.Add(accountTransaction);
        }

        return accountTransactions;
    }

    private List<AccountFastTransactionDto> MapToAccountFastTransactions(List<FastTransaction> fastTransactions, CurrencyDto currency)
    {
        var accountFastTransactions = new List<AccountFastTransactionDto>();

        return accountFastTransactions;
    }

    private List<CreditCardDto> MapToAccountCreditCards(List<CreditCard> creditCards, CurrencyDto currency)
    {
        var accountCreditCards = new List<CreditCardDto>();

        return accountCreditCards;
    }

    private List<DebitCardDto> MapToAccountDebitCards(List<DebitCard> debitCards, CurrencyDto currency)
    {
        var accountDebitCards = new List<DebitCardDto>();

        return accountDebitCards;
    }

    private Money CreateMoney(decimal amount, CurrencyDto currency) =>
        new(amount, currency);

    private string CreateFullName(Customer customer) =>
    customer.FirstName + " " + customer.LastName;

    #endregion
}
