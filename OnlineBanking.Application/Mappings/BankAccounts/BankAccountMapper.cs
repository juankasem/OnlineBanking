
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
        var currency = MapToAccountCurrencyDTO(bankAccount.Currency);

        return new(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                        MapToAccountBranchDTO(bankAccount.Branch),
                                        MapToAccountBalanceDTO(bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                                            bankAccount.MinimumAllowedBalance, bankAccount.Debt),
                                        currency);
    }

    public BankAccountResponse MapToResponseModel(BankAccount bankAccount, IReadOnlyList<CashTransaction> accountTransactions)
    {
        var currency = MapToAccountCurrencyDTO(bankAccount.Currency);

        return new(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                        MapToAccountBranchDTO(bankAccount.Branch),
                                        MapToAccountBalanceDTO(bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                                            bankAccount.MinimumAllowedBalance, bankAccount.Debt),
                                        currency,
                                        MapToAccountOwnersDTO(bankAccount.BankAccountOwners.ToList()),
                                        MapToAccountTransactionsDTO(accountTransactions, currency),
                                        MapToAccountFastTransactionsDTO(bankAccount.FastTransactions.ToList(), currency),
                                        MapToAccountCreditCardsDTO(bankAccount.CreditCards.ToList(), currency),
                                        MapToAccountDebitCardsDTO(bankAccount.DebitCards.ToList(), currency));
    }

    #region Private helper methods
    private CurrencyDto MapToAccountCurrencyDTO(Currency currency) =>
        new(currency.Id, currency.Code, currency.Name, currency.Symbol);
    
    private BranchDto MapToAccountBranchDTO(Branch branch) =>
        new(branch.Id, branch.Name);

    private AccountBalanceDto MapToAccountBalanceDTO(decimal balance, decimal allowedBalanceToUse,
                                                    decimal minimumAllowedBalance, decimal debt ) =>
        new (balance, allowedBalanceToUse, minimumAllowedBalance, debt);

    private List<AccountOwnerDto> MapToAccountOwnersDTO(List<CustomerBankAccount> customerBankAccounts)
    {
        var bankAccountOwners = new List<AccountOwnerDto>();

        foreach (var customerBankAccount in customerBankAccounts)
        {
            var bankAccountOwner = new AccountOwnerDto(customerBankAccount.Customer.Id, CreateFullName(customerBankAccount.Customer));

            bankAccountOwners.Add(bankAccountOwner);
        }
        return bankAccountOwners;
    }

    private List<AccountTransactionDto> MapToAccountTransactionsDTO(IReadOnlyList<CashTransaction> accountTransactions, CurrencyDto currency) =>
           
            accountTransactions.Select(tx => new AccountTransactionDto(tx.Type, tx.InitiatedBy,
                                                                CreateMoney(tx.Amount, currency), CreateMoney(tx.Fees, currency),
                                                                tx.Description, tx.PaymentType, tx.TransactionDate, tx.Status,
                                                                tx.From, tx.To, tx.Sender, tx.Recipient)).ToList();
    

    private List<AccountFastTransactionDto> MapToAccountFastTransactionsDTO(List<FastTransaction> fastTransactions, CurrencyDto currency)
    {
        var accountFastTransactions = new List<AccountFastTransactionDto>();

        return accountFastTransactions;
    }

    private List<CreditCardDto> MapToAccountCreditCardsDTO(List<CreditCard> creditCards, CurrencyDto currency)
    {
        var accountCreditCards = new List<CreditCardDto>();

        return accountCreditCards;
    }

    private List<DebitCardDto> MapToAccountDebitCardsDTO(List<DebitCard> debitCards, CurrencyDto currency)
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
