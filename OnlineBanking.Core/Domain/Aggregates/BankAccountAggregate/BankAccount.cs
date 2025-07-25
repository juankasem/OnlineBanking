using System.Text.Json.Serialization;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class BankAccount : BaseDomainEntity
{
    private readonly List<CustomerBankAccount> _bankAccountOwners = [];
    private readonly List<AccountTransaction> _accountTransactions = [];
    private readonly List<FastTransaction> _fastTransactions = [];
    private readonly List<CreditCard> _creditCards = [];
    private readonly List<DebitCard> _debitCards = [];

    /// <summary>
    /// Account Number
    /// </summary>        
    public string AccountNo { get; private set; }

    /// <summary>
    /// IBAN
    /// </summary>  
    public string IBAN { get; private set; }

    /// <summary>
    /// Bank Account Type
    /// </summary>  
    public BankAccountType Type { get; private set; }

    /// <summary>
    /// Bank Account Branch
    /// </summary>
    public int BranchId { get; private set; }
    public Branch Branch { get; private set; }

    /// <summary>
    /// Bank Account Balance
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Bank Account allowed balance to use
    /// </summary>
    public decimal AllowedBalanceToUse { get; private set; }

    /// <summary>
    /// Bank Account minimum allowed balance 
    /// </summary>
    public decimal MinimumAllowedBalance { get; private set; }

    /// <summary>
    /// Bank Account debt 
    /// </summary>
    public decimal Debt { get; private set; }

    /// <summary>
    /// Bank Account Currency
    /// </summary>
    public int CurrencyId { get; private set; }
    public Currency Currency { get; private set; }

    public bool IsActive { get; private set; }

    // Many-to-many relationships
    [JsonIgnore]
    public IReadOnlyList<CustomerBankAccount> BankAccountOwners { get { return _bankAccountOwners.AsReadOnly(); } }

    [JsonIgnore]
    public IReadOnlyList<AccountTransaction> AccountTransactions { get { return _accountTransactions.AsReadOnly(); } }

    // One-to-Many relationships
    public IReadOnlyList<FastTransaction> FastTransactions { get { return _fastTransactions.AsReadOnly(); } }
    public IReadOnlyList<CreditCard> CreditCards { get { return _creditCards.AsReadOnly(); } }
    public IReadOnlyList<DebitCard> DebitCards { get { return _debitCards.AsReadOnly(); } }

    private BankAccount(Guid id, string accountNo, string iBAN, BankAccountType type,
                        int branchId, decimal balance, decimal allowedBalanceToUse,
                        decimal minimumAllowedBalance, decimal debt,
                        int currencyId, bool isActive = false, bool isDeleted = false)
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

    //Factories
    /// <summary>
    /// Creates a new bank account instance
    /// </summary>
    /// <param name="id">Bank account ID</param>
    /// <param name="accountNo">Account No</param>
    /// <param name="iban">IBAN</param>
    /// <returns><see cref="BankAccount"/></returns>
    /// <exception cref="BankAccountNotValidException"></exception>
    public static BankAccount Create(string accountNo, string iban, BankAccountType type,
                                    int branchId, decimal balance, decimal allowedBalanceToUse,
                                    decimal minimumAllowedBalance, decimal debt, int currencyId,
                                    bool isActive = true, bool isDeleted = false, Guid? id = null)
    {
        var validator = new BankAccountValidator();

        var objectToValidate = new BankAccount(
            id ?? Guid.NewGuid(),
            accountNo,
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

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new BankAccountNotValidException("Bank Account is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }

    public void AddOwnerToBankAccount(CustomerBankAccount customerBankAccount) =>
                                    _bankAccountOwners.Add(customerBankAccount);

    public void AddTransaction(AccountTransaction at) => _accountTransactions.Add(at);

    public void UpdateTransaction(CashTransaction cashTransaction)
    {
        var accountTransaction = _accountTransactions.FirstOrDefault(at => at.Transaction.Id == cashTransaction.Id);

        if (accountTransaction is not null)
            accountTransaction.Transaction = cashTransaction;
    }

    public void DeleteTransaction(Guid id) 
    {
        var index = _accountTransactions.FindIndex(c => c.Transaction?.Id == id);

        if (index >= 0) _accountTransactions.Remove(_accountTransactions[index]);
    }

    public void AddFastTransaction(FastTransaction ft) => _fastTransactions.Add(ft);

    public void UpdateFastTransaction(Guid id, FastTransaction ft)
    {
        var index = _fastTransactions.FindIndex(ct => ct.Id == id);

        if (index >= 0) _fastTransactions[index] = ft;
    }

    public void DelteFastTransaction(Guid id)
    {
        var index = _fastTransactions.FindIndex(c => c.Id == id);

        if (index >= 0) _fastTransactions.Remove(_fastTransactions[index]);
    }

    public void AddCreditCard(CreditCard creditCard) => _creditCards.Add(creditCard);

    public void UpdateCreditCard(Guid id, CreditCard creditCard)
    {
        var index = _creditCards.FindIndex(c => c.Id == id);

        if (index >= 0) _creditCards[index] = creditCard;
    }

    public void ActivateCreditCard(Guid creditCardId)
    {
        var creditCard = _creditCards.FirstOrDefault(c => c.Id == creditCardId);

        if (creditCard is not null) creditCard.Activate();
    }

    public void SetPINToCreditCard(Guid creditCardId, string pIN)
    {
        var creditCard = _creditCards.FirstOrDefault(c => c.Id == creditCardId);

        if (creditCard is not null && pIN.Length == 4)
            creditCard.SetPIN(pIN);
    }

    public void AddDebitCard(DebitCard debitCard) => _debitCards.Add(debitCard);

    public void UpdateDebitCard(Guid id, DebitCard debitCard)
    {
        var index = _debitCards.FindIndex(c => c.Id == id);

        if (index >= 0) _debitCards[index] = debitCard;
    }


    public decimal UpdateBalance(decimal amount, string operationType)
    {
        if (operationType == OperationType.Add)
            Balance += amount;
        else
            Balance -= amount;

        AllowedBalanceToUse = Balance;

        return Balance;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}