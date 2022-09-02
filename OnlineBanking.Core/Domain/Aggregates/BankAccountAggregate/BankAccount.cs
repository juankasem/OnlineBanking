using System;
using System.Collections.Generic;
using System.Linq;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
public class BankAccount : BaseDomainEntity
{
    private readonly List<CustomerBankAccount> _bankAccountOwners = new List<CustomerBankAccount>();
    private readonly List<CashTransaction> _cashTransactions = new List<CashTransaction>();
    private readonly List<FastTransaction> _fastTransactions = new List<FastTransaction>();
    private readonly List<CreditCard> _creditCards = new List<CreditCard>();
    private readonly List<DebitCard> _debitCards = new List<DebitCard>();

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

    // Many-to-many relationship
    public ICollection<CustomerBankAccount> BankAccountOwners { get { return _bankAccountOwners; } }
    
    // One-to-Many relationship
    public ICollection<CashTransaction> CashTransactions { get { return _cashTransactions; } }
    public ICollection<FastTransaction> FastTransactions { get { return _fastTransactions; } }
    public ICollection<CreditCard> CreditCards { get { return _creditCards; } }
    public ICollection<DebitCard> DebitCards { get { return _debitCards; } }

    private BankAccount(Guid id, string accountNo, string iban, BankAccountType type,
                        int branchId, decimal balance, decimal allowedBalanceToUse,
                        decimal minimumAllowedBalance, decimal debt,
                        int currencyId, bool isActive = false, bool isDeleted = false)
    {
        Id = id;
        AccountNo = accountNo;
        IBAN = iban;
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

    public void AddCashTransaction(CashTransaction ct) => _cashTransactions.Add(ct);

    public void UpdateCashTransaction(Guid id, CashTransactionStatus status)
    {
        var cashTransaction = _cashTransactions.FirstOrDefault(ct => ct.Id == id);

        if (cashTransaction != null)
            cashTransaction.Update(status);
    }

    public void DeleteCashTransaction(CashTransaction ct) => _cashTransactions.Remove(ct);

    public void AddFastTransaction(FastTransaction ft) => _fastTransactions.Add(ft);

    public void UpdateFastTransaction(Guid id, FastTransaction ft)
    {
        var index = _fastTransactions.FindIndex(ct => ct.Id == id);

        if (index >= 0)
        _fastTransactions[index] = ft;
    }

    public void DelteFastTransaction(FastTransaction ft) => _fastTransactions.Remove(ft);

    public void AddCreditCard(CreditCard creditCard) => _creditCards.Add(creditCard);

    public void AddDebitCard(DebitCard debitCard) => _debitCards.Add(debitCard);

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