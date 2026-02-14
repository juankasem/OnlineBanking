using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;
using OnlineBanking.Core.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

/// <summary>
/// Represents a bank account aggregate root.
/// Manages account state, balance, associated cards, transactions, and owners.
/// </summary>
public class BankAccount : AggregateRoot<Guid> 
{
    #region Private Fields

    private readonly List<CustomerBankAccount> _bankAccountOwners = [];
    private readonly List<AccountTransaction> _accountTransactions = [];
    private readonly List<FastTransaction> _fastTransactions = [];
    private readonly List<CreditCard> _creditCards = [];
    private readonly List<DebitCard> _debitCards = [];

    #endregion

    #region Properties

    /// <summary>
    /// Unique account number
    /// </summary>     
    public string AccountNo { get; private set; }

    /// <summary>
    /// International Bank Account Number (IBAN)
    /// </summary>
    public string IBAN { get; private set; }

    /// <summary>
    /// Type of bank account (Savings, Current, Deposit, Joint)
    /// </summary>
    public BankAccountType Type { get; private set; }

    /// <summary>
    /// ID of the branch managing this account
    /// </summary>
    public int BranchId { get; private set; }

    /// <summary>
    /// Navigation property to the managing branch
    /// </summary>
    public Branch Branch { get; private set; }

    /// <summary>
    /// Current account balance
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Maximum balance available for withdrawal/transfer
    /// </summary>
    public decimal AllowedBalanceToUse { get; private set; }

    /// <summary>
    /// Minimum balance required to maintain in the account
    /// </summary>
    public decimal MinimumAllowedBalance { get; private set; }

    /// <summary>
    /// Outstanding debt on the account
    /// </summary>
    public decimal Debt { get; private set; }

    /// <summary>
    /// Currency ID for the account
    /// </summary>
    public int CurrencyId { get; private set; }

    /// <summary>
    /// Navigation property to the account currency
    /// </summary>
    public Currency Currency { get; private set; }

    /// <summary>
    /// Account activation status
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Account owners (many-to-many relationship)
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<CustomerBankAccount> BankAccountOwners => _bankAccountOwners.AsReadOnly();

    /// <summary>
    /// Account transactions (many-to-many relationship)
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<AccountTransaction> AccountTransactions => _accountTransactions.AsReadOnly();

    /// <summary>
    /// Fast transactions (one-to-many relationship)
    /// </summary>
    public IReadOnlyList<FastTransaction> FastTransactions => _fastTransactions.AsReadOnly();

    /// <summary>
    /// Credit cards (one-to-many relationship)
    /// </summary>
    public IReadOnlyList<CreditCard> CreditCards => _creditCards.AsReadOnly();

    /// <summary>
    /// Debit cards (one-to-many relationship)
    /// </summary>
    public IReadOnlyList<DebitCard> DebitCards => _debitCards.AsReadOnly();

    #endregion

    #region Constructor

    private BankAccount(
        Guid id, 
        string accountNo, 
        string iBAN, 
        BankAccountType type,
        int branchId, 
        decimal balance, 
        decimal allowedBalanceToUse,
        decimal minimumAllowedBalance, 
        decimal debt,
        int currencyId, 
        bool isActive = false, 
        bool isDeleted = false)
    {
        Id = id;
        AccountNo = accountNo;
        IBAN = iBAN;
        Type = type;
        BranchId = branchId;
        Balance = balance;
        AllowedBalanceToUse = allowedBalanceToUse;
        MinimumAllowedBalance = minimumAllowedBalance;
        Debt = debt;
        CurrencyId = currencyId;
        IsActive = isActive;
        IsDeleted = isDeleted;
    }

    #endregion

    #region Factory Method

    /// <summary>
    /// Creates a new bank account instance
    /// </summary>
    /// <param name="id">Bank account ID</param>
    /// <param name="accountNo">Account No</param>
    /// <param name="iban">IBAN</param>
    /// <returns><see cref="BankAccount"/></returns>
    /// <exception cref="BankAccountNotValidException"></exception>
    public static BankAccount Create(
        string accountNo, 
        string iban, 
        BankAccountType type,
        int branchId, 
        decimal balance, 
        decimal allowedBalanceToUse,
        decimal minimumAllowedBalance, 
        decimal debt, 
        int currencyId,
        bool isActive = true, 
        bool isDeleted = false, 
        Guid? id = null)
    {
        var validator = new BankAccountValidator();

        var bankAccount = new BankAccount(
            id ?? Guid.NewGuid(),
            string.IsNullOrEmpty(accountNo) ? GenerateAccountNo() : accountNo,
            iban,
            type,
            branchId,
            balance,
            allowedBalanceToUse,
            minimumAllowedBalance,
            debt,
            currencyId,
            isActive,
            isDeleted);

        bankAccount.IBAN ??= "DE34" + bankAccount.AccountNo;

        var validationResult = validator.Validate(bankAccount);

        if (validationResult.IsValid)
        {
            bankAccount.AddDomainEvent(new BankAccountCreatedEvent(
                bankAccount.Id,
                bankAccount.AccountNo,
                bankAccount.IBAN,
                bankAccount.Type,
                bankAccount.BranchId,
                bankAccount.Balance,
                bankAccount.CurrencyId));

            return bankAccount;
        }
        var exception = new BankAccountNotValidException("Bank Account is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }

    private static string GenerateAccountNo()
    {
        var random = Random.Shared;
        string accountNumber = random.Next(0, 100_000_000).ToString("D8") + 
                               random.Next(0, 100_000_000).ToString("D8") + 
                               random.Next(0, 100).ToString("D2");

        return accountNumber;
    }

    #endregion

    #region Account Owners

    /// <summary>
    /// Adds an owner to this bank account
    /// </summary>
    public void AddOwnerToBankAccount(
        CustomerBankAccount customerBankAccount) => _bankAccountOwners.Add(customerBankAccount);
    #endregion

    #region Account Transactions

    /// <summary>
    /// Adds a cash transaction to this account.
    /// Validates transaction and prevents duplicates.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when transaction is null</exception>
    /// <exception cref="ArgumentException">Thrown when transaction has invalid ID</exception>
    /// <exception cref="InvalidOperationException">Thrown when currency mismatches or duplicate exists</exception>
    public void AddAccountTransaction(CashTransaction cashTransaction)
    {
        ArgumentNullException.ThrowIfNull(cashTransaction);

        if (cashTransaction.Id == Guid.Empty) 
            throw new ArgumentException("Transaction must have a valid Id.", 
                nameof(cashTransaction));

        // avoid adding the same transaction twice
        if (_accountTransactions.Any(at => at.TransactionId == cashTransaction.Id))
            return;

        // ensure transaction currency matches account currency
        if (cashTransaction.CurrencyId != CurrencyId)
            throw new InvalidOperationException("Transaction currency does not match account currency.");

        var accountTransaction = AccountTransaction.Create(Id, cashTransaction.Id);

        accountTransaction.Account = this;
        accountTransaction.Transaction = cashTransaction;

        _accountTransactions.Add(accountTransaction);
    }

    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    public void UpdateTransaction(CashTransaction cashTransaction)
    {
        var accountTransaction = _accountTransactions.FirstOrDefault(at => at.Transaction.Id == cashTransaction.Id);

        if (accountTransaction is not null)
            accountTransaction.Transaction = cashTransaction;
    }

    /// <summary>
    /// Removes a transaction by ID
    /// </summary>
    public void DeleteTransaction(Guid id)
    {
        var index = _accountTransactions.FindIndex(c => c.Transaction?.Id == id);

        if (index >= 0) 
            _accountTransactions.Remove(_accountTransactions[index]);
    }

    #endregion

    #region Fast Transactions

    /// <summary>
    /// Adds a fast transaction to this account
    /// </summary>
    public void AddFastTransaction(FastTransaction ft) => _fastTransactions.Add(ft);

    /// <summary>
    /// Updates an existing fast transaction
    /// </summary>
    public void UpdateFastTransaction(Guid id, FastTransaction ft)
    {
        var index = _fastTransactions.FindIndex(ct => ct.Id == id);

        if (index >= 0) 
            _fastTransactions[index] = ft;
    }

    /// <summary>
    /// Removes a fast transaction by ID
    /// </summary>
    public void DeleteFastTransaction(Guid id)
    {
        var index = _fastTransactions.FindIndex(c => c.Id == id);

        if (index >= 0) 
            _fastTransactions.Remove(_fastTransactions[index]);
    }

    #endregion

    #region Credit Cards

    /// <summary>
    /// Adds a credit card to this account
    /// </summary>
    public void AddCreditCard(CreditCard creditCard) => _creditCards.Add(creditCard);

    /// <summary>
    /// Updates an existing credit card
    /// </summary>
    public void UpdateCreditCard(Guid id, CreditCard creditCard)
    {
        var index = _creditCards.FindIndex(c => c.Id == id);

        if (index >= 0) 
            _creditCards[index] = creditCard;
    }

    /// <summary>
    /// Activates a credit card
    /// </summary>
    public void ActivateCreditCard(Guid creditCardId)
    {
        var creditCard = _creditCards.FirstOrDefault(c => c.Id == creditCardId);

        creditCard?.Activate();
    }

    /// <summary>
    /// Sets PIN for a credit card (must be exactly 4 digits)
    /// </summary>
    public void SetPINToCreditCard(Guid creditCardId, string pIN)
    {
        var creditCard = _creditCards.FirstOrDefault(c => c.Id == creditCardId);

        if (creditCard is not null && pIN.Length == 4)
            creditCard.SetPIN(pIN);
    }

    #endregion

    #region Debit Cards

    /// <summary>
    /// Adds a debit card to this account
    /// </summary>
    public void AddDebitCard(DebitCard debitCard) => _debitCards.Add(debitCard);

    /// <summary>
    /// Updates an existing debit card
    /// </summary>
    public void UpdateDebitCard(Guid id, DebitCard debitCard)
    {
        var index = _debitCards.FindIndex(c => c.Id == id);

        if (index >= 0) 
            _debitCards[index] = debitCard;
    }

    #endregion


    #region Balance Operations

    /// <summary>
    /// Updates account balance based on deposit or withdrawal
    /// </summary>
    /// <param name="amount">Amount to add or subtract</param>
    /// <param name="isDeposit">True for deposit, false for withdrawal</param>
    /// <returns>Updated balance</returns>
    public decimal UpdateBalance(decimal amount, bool isDeposit = true)
    {
        if (isDeposit)
            Balance = decimal.Round(Balance + amount, 2);
        else
            Balance = decimal.Round(Balance - amount, 2);

        AllowedBalanceToUse = Balance;
        return Balance;
    }

    /// <summary>
    /// Deposits funds into the account
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when amount is null</exception>
    /// <exception cref="ArgumentException">Thrown when amount is negative</exception>
    public void Deposit(Money amount)
    {
        ArgumentNullException.ThrowIfNull(amount);

        if (amount.IsNegative()) 
            throw new ArgumentException("Amount must be positive", nameof(amount));

        // if you switched to Money for Balance, use Money arithmetic. Example uses decimal for compatibility:
        Balance = decimal.Round(Balance + amount.Amount, 2);
        AllowedBalanceToUse = Balance;
        // TODO: Raise domain event: new BalanceChangedEvent(Id, Balance, BalanceChangeType.Deposit)
    }

    /// <summary>
    /// Withdraws funds from the account
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when amount is null</exception>
    /// <exception cref="ArgumentException">Thrown when amount is negative</exception>
    /// <exception cref="InsufficientFundsException">Thrown when withdrawal would violate minimum balance</exception>
    public void Withdraw(Money amount)
    {
        ArgumentNullException.ThrowIfNull(amount);

        if (amount.IsNegative()) 
            throw new ArgumentException("Amount must be positive", nameof(amount));

        var newBalance = decimal.Round(Balance - amount.Amount, 2);

        if (newBalance < MinimumAllowedBalance)
            throw new InsufficientFundsException("Withdrawal would violate minimum allowed balance.");

        Balance = newBalance;
        AllowedBalanceToUse = Balance;
        // raise domain event here: new BalanceChangedEvent(Id, Balance)
    }

    #endregion

    #region Status Operations

    /// <summary>
    /// Activates the account
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// Deactivates the account
    /// </summary>
    public void Deactivate() => IsActive = false;

    #endregion
}