using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.Currency;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Application.Models.DebitCard;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Mappings.BankAccounts;

/// <summary>
/// Mapper for converting BankAccount domain entities to DTOs and response models.
/// Handles mapping of account details, owners, transactions, and associated cards.
/// </summary>
public class BankAccountMapper : IBankAccountMapper
{
    private const string FullNameSeparator = " ";
    private const string UnknownCustomer = "Unknown";

    /// <summary>
    /// Maps a bank account to a basic DTO model.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when bankAccount is null</exception>
    public BankAccountDto MapToDtoModel(BankAccount bankAccount)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);

        var currency = MapToCurrencyDto(bankAccount.Currency);

        return new(
            bankAccount.AccountNo, 
            bankAccount.IBAN,
            MapToAccountOwnersDto(bankAccount.BankAccountOwners.Select(b => b.Customer)
            .ToList()
            .AsReadOnly()),
            bankAccount.Type,
            MapToBranchDto(bankAccount.Branch),
            MapToBalanceDto(bankAccount),
            currency);
    }

    /// <summary>
    /// Maps a bank account to a comprehensive response model with all related data.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    public BankAccountResponse MapToResponseModel(
        BankAccount bankAccount,
        IReadOnlyList<Customer> bankAccountOwners,
        IReadOnlyList<CashTransaction> cashTransactions)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        ArgumentNullException.ThrowIfNull(bankAccountOwners);
        ArgumentNullException.ThrowIfNull(cashTransactions);

        var currency = MapToCurrencyDto(bankAccount.Currency);

        return new(
            bankAccount.AccountNo, 
            bankAccount.IBAN, 
            bankAccount.Type,
            MapToBranchDto(bankAccount.Branch),
            MapToBalanceDto(bankAccount),
            currency,
            MapToAccountOwnersDto(bankAccountOwners),
            MapToAccountTransactionsDto(cashTransactions, currency),
            MapToFastTransactionsDto(bankAccount.FastTransactions, currency),
            MapToCreditCardsDto(bankAccount.CreditCards),
            MapToDebitCardsDto(bankAccount.DebitCards));
    }

    #region Private helper methods

    /// <summary>
    /// Maps currency entity to DTO.
    /// </summary>
    private static CurrencyDto MapToCurrencyDto(Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        return new(
            currency.Id,
            currency.Code,
            currency.Name,
            currency.Symbol
        );
    }

    /// <summary>
    /// Maps branch entity to DTO.
    /// </summary>
    private static BranchDto MapToBranchDto(Branch branch)
    {
        ArgumentNullException.ThrowIfNull(branch);

        return new(branch.Id, branch.Name);
    }

    /// <summary>
    /// Maps bank account balance information to DTO.
    /// </summary>
    private static AccountBalanceDto MapToBalanceDto(BankAccount bankAccount)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);

        return new(
             bankAccount.Balance,
             bankAccount.AllowedBalanceToUse,
             bankAccount.MinimumAllowedBalance,
             bankAccount.Debt);   
    }

    /// <summary>
    /// Maps account owners to DTOs using LINQ for better readability.
    /// </summary>
    private static AccountOwnerDto[] MapToAccountOwnersDto(IReadOnlyList<Customer> customers)
    {
        ArgumentNullException.ThrowIfNull(customers);

        if (customers.Count == 0)
        {
            return [];
        }

        var ownersDto = customers
            .Where(customer => customer is not null)
            .Select(customer => new AccountOwnerDto(
                customer.Id,
                customer.CustomerNo,
                CreateFullName(customer)))
            .ToArray();

        return ownersDto;
    }

    /// <summary>
    /// Maps cash transactions to DTOs.
    /// </summary>
    private static AccountTransactionDto[] MapToAccountTransactionsDto(
        IReadOnlyList<CashTransaction> cashTransactions, 
        CurrencyDto currency)
    {
        ArgumentNullException.ThrowIfNull(cashTransactions);
        ArgumentNullException.ThrowIfNull(currency);

        if (cashTransactions.Count == 0)
        {
            return [];
        }

        var accountTransactions = cashTransactions
                   .Where(ct => ct is not null)
                   .Select(ct => new AccountTransactionDto(
                       Enum.GetName(ct.Type),
                       Enum.GetName(ct.InitiatedBy),
                       CreateMoney(ct.Amount, currency),
                       CreateMoney(ct.Fees, currency),
                       ct.Description,
                       Enum.GetName(ct.PaymentType),
                       ct.TransactionDate,
                       Enum.GetName(ct.Status),
                       ct.From,
                       ct.To,
                       ct.Sender,
                       ct.Recipient))
                   .ToArray();

        return accountTransactions;
    }

    /// <summary>
    /// Maps fast transactions to DTOs.
    /// </summary>
    private static AccountFastTransactionDto[] MapToFastTransactionsDto(
        IReadOnlyList<FastTransaction> fastTransactions, 
        CurrencyDto currency)
    {
        ArgumentNullException.ThrowIfNull(fastTransactions);
        ArgumentNullException.ThrowIfNull(currency);

        if (fastTransactions.Count == 0)
        {
            return [];
        }

        var fastTransactionsDto = fastTransactions
          .Where(ft => ft is not null)
          .Select(ft => new AccountFastTransactionDto(
              ft.BankAccount?.IBAN ?? string.Empty,
              ft.RecipientIBAN,
              ft.RecipientName,
              ft.BankAccount?.Branch?.Name ?? string.Empty,
              CreateMoney(ft.Amount, currency)))
          .ToArray();

        return fastTransactionsDto;
    }

    /// <summary>
    /// Maps credit cards to DTOs.
    /// Extracts relevant credit card information for API response.
    /// </summary>
    private static CreditCardDto[] MapToCreditCardsDto(
        IReadOnlyList<CreditCard> creditCards)
    {
        ArgumentNullException.ThrowIfNull(creditCards);

        if (creditCards.Count == 0)
        {
            return [];
        }

        var creditCardsDto = creditCards
            .Where(cc => cc is not null)
            .Select(cc => new CreditCardDto(
                CreateFullName(cc.BankAccount.BankAccountOwners[0].Customer),
                MaskCardNumber(cc.CreditCardNo),
                cc.CustomerNo,
                cc.ValidTo,
                cc.SecurityCode))
            .ToArray();

        return creditCardsDto;
    }

    /// <summary>
    /// Maps debit cards to DTOs.
    /// Extracts relevant debit card information for API response.
    /// </summary>
    private static DebitCardDto[] MapToDebitCardsDto(
        IReadOnlyList<DebitCard> debitCards)
    {
        ArgumentNullException.ThrowIfNull(debitCards);

        if (debitCards.Count == 0)
        {
            return [];
        }

        var debitCardsDto = debitCards
                 .Where(cc => cc is not null)
                 .Select(cc => new DebitCardDto(
                     CreateFullName(cc.BankAccount.BankAccountOwners[0].Customer),
                     MaskCardNumber(cc.DebitCardNo),
                     cc.CustomerNo,
                     cc.ValidTo,
                     cc.SecurityCode))
                 .ToArray();

        return debitCardsDto;
    }

    /// <summary>
    /// Creates a Money value object from amount and currency.
    /// </summary>
    private static Money CreateMoney(decimal amount, CurrencyDto currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        return new Money(amount, currency); 
    }

    /// <summary>
    /// Creates a full name from customer's first and last name.
    /// Returns "Unknown" if customer is null or both names are empty.
    /// </summary>
    private static string CreateFullName(Customer? customer)
    {
        if (customer is null)
        {
            return UnknownCustomer;
        }

        var firstName = customer.FirstName?.Trim() ?? string.Empty;
        var lastName = customer.LastName?.Trim() ?? string.Empty;

        return string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName)
            ? UnknownCustomer
            : $"{firstName}{FullNameSeparator}{lastName}".Trim();
    }

    /// <summary>
    /// Masks sensitive card number data for security.
    /// Shows only the last 4 digits: XXXX-XXXX-XXXX-1234
    /// </summary>
    private static string MaskCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
        {
            return "****-****-****-****";
        }

        var lastFourDigits = cardNumber[^4..];
        return $"****-****-****-{lastFourDigits}";
    }
    #endregion
}
