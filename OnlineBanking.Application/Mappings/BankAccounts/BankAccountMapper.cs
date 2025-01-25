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

    public BankAccountResponse MapToResponseModel(BankAccount bankAccount,
                                                 IReadOnlyList<Customer> bankAccountOwners,
                                                 IReadOnlyList<CashTransaction> cashTransactions)
    {
        var currency = MapToAccountCurrencyDTO(bankAccount.Currency);

        return new(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                        MapToAccountBranchDTO(bankAccount.Branch),
                                        MapToAccountBalanceDTO(bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                                            bankAccount.MinimumAllowedBalance, bankAccount.Debt),
                                        currency,
                                        MapToAccountOwnersDTO(bankAccountOwners),
                                        MapToAccountTransactionsDTO(cashTransactions, currency),
                                        MapToAccountFastTransactionsDTO(bankAccount.FastTransactions, currency),
                                        MapToAccountCreditCardsDTO(bankAccount.CreditCards, currency),
                                        MapToAccountDebitCardsDTO(bankAccount.DebitCards, currency));
    }

    #region Private helper methods
    private static CurrencyDto MapToAccountCurrencyDTO(Currency currency) =>
        new(currency.Id, currency.Code, currency.Name, currency.Symbol);
    
    private static BranchDto MapToAccountBranchDTO(Branch branch) =>
        new(branch.Id, branch.Name);

    private static AccountBalanceDto MapToAccountBalanceDTO(decimal balance, decimal allowedBalanceToUse,
                                                    decimal minimumAllowedBalance, decimal debt ) =>
        new (balance, allowedBalanceToUse, minimumAllowedBalance, debt);

    private IReadOnlyList<AccountOwnerDto> MapToAccountOwnersDTO(IReadOnlyList<Customer> customers)
    {
        var bankAccountOwners = new List<AccountOwnerDto>();

        if (customers == null || customers.Count == 0)
            return bankAccountOwners.AsReadOnly(); 

        foreach (var customer in customers)
        {
            var bankAccountOwner = new AccountOwnerDto(customer.Id, customer.CustomerNo, CreateFullName(customer));

            bankAccountOwners.Add(bankAccountOwner);
        }

        return bankAccountOwners.AsReadOnly();
    }

    private IReadOnlyList<AccountTransactionDto> MapToAccountTransactionsDTO(IReadOnlyList<CashTransaction> cashTransactions, CurrencyDto currency)
    {
        var accountTransactions = new List<AccountTransactionDto>();

        if (cashTransactions == null || cashTransactions.Count == 0)
        return accountTransactions.AsReadOnly();

        foreach (var ct in cashTransactions)
        {
            var accountTransaction = new AccountTransactionDto(Enum.GetName(typeof(Core.Domain.Enums.CashTransactionType), ct.Type),
                                                                Enum.GetName(typeof(Core.Domain.Enums.BankAssetType), ct.InitiatedBy),
                                                                CreateMoney(ct.Amount, currency), CreateMoney(ct.Fees, currency),
                                                                ct.Description,
                                                                Enum.GetName(typeof(Core.Domain.Enums.PaymentType), ct.PaymentType),
                                                                ct.TransactionDate,
                                                                Enum.GetName(typeof(Core.Domain.Enums.CashTransactionStatus), ct.Status),
                                                                ct.From, ct.To, ct.Sender, ct.Recipient);
            accountTransactions.Add(accountTransaction);
        }
        
        return accountTransactions.AsReadOnly();
    }
        

    private IReadOnlyList<AccountFastTransactionDto> MapToAccountFastTransactionsDTO(IReadOnlyList<FastTransaction> fastTransactions, CurrencyDto currency)
    {
        var accountFastTransactions = new List<AccountFastTransactionDto>();

        if (fastTransactions == null || fastTransactions.Count == 0)
        return accountFastTransactions.AsReadOnly();

        foreach (var ft in fastTransactions)
        {
            var accountFastTransaction = new AccountFastTransactionDto(ft.BankAccount.IBAN, ft.RecipientIBAN, ft.RecipientName, 
                                                                       ft.BankAccount.Branch.Name, CreateMoney(ft.Amount, currency));

            accountFastTransactions.Add(accountFastTransaction);
        }

        return accountFastTransactions.AsReadOnly();
    }

    private IReadOnlyList<CreditCardDto> MapToAccountCreditCardsDTO(IReadOnlyList<CreditCard> creditCards, CurrencyDto currency)
    {
        var accountCreditCards = new List<CreditCardDto>();

        return accountCreditCards;
    }

    private IReadOnlyList<DebitCardDto> MapToAccountDebitCardsDTO(IReadOnlyList<DebitCard> debitCards, CurrencyDto currency)
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
