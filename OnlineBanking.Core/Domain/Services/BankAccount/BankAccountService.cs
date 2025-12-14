
using Microsoft.Extensions.Logging;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Core.Domain.Services.BankAccount;

public class BankAccountService(ILogger<BankAccountService> logger) : IBankAccountService
{
    private readonly ILogger<BankAccountService> _logger = logger;

    public bool CreateCashTransaction(Aggregates.BankAccountAggregate.BankAccount senderAccount,
                                      Aggregates.BankAccountAggregate.BankAccount recipientAccount,
                                      CashTransaction cashTransaction,
                                      decimal fees = 0)
    {
        ArgumentNullException.ThrowIfNull(cashTransaction);
        if (cashTransaction.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(cashTransaction));

        _logger.LogDebug("Applying cash transaction {TransactionId} type={Type}", cashTransaction.Id, cashTransaction.Type);
        
        var amount = cashTransaction.Amount;

        // Modern C# switch expression with pattern matching
        switch (cashTransaction.Type)
        {
            case CashTransactionType.Transfer:
            case CashTransactionType.FAST:
                ApplyTransfer(senderAccount, recipientAccount, cashTransaction, amount, fees);
                break;

            case CashTransactionType.Deposit:
                ApplyDeposit(recipientAccount, cashTransaction, amount);
                break;

            case CashTransactionType.Withdrawal:
                ApplyWithdrawal(senderAccount, cashTransaction, amount, fees);
                break;

            default:
                throw new InvalidOperationException($"Unsupported transaction type: {cashTransaction.Type}");
        }

        return true;
    }

    public bool CreateCustomer(Aggregates.BankAccountAggregate.BankAccount bankAccount, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        ArgumentNullException.ThrowIfNull(customer);

        bankAccount.AddOwnerToBankAccount(CustomerBankAccount.Create(bankAccount.Id, customer.Id));
        return true;
    }

    public bool CreateFastTransaction(Aggregates.BankAccountAggregate.BankAccount bankAccount, FastTransaction fastTransaction)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        ArgumentNullException.ThrowIfNull(fastTransaction);
        if (fastTransaction.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(fastTransaction));

        bankAccount.AddFastTransaction(fastTransaction);
        return true;
    }

    public bool DeleteFastTransation(Guid fastTransactionId, Aggregates.BankAccountAggregate.BankAccount bankAccount)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        if (fastTransactionId == Guid.Empty) throw new ArgumentException("Invalid fast transaction id.", nameof(fastTransactionId));

        bankAccount.DeleteFastTransaction(fastTransactionId);
        return true;
    }

    #region Transaction Operations

    private static void ApplyTransfer(Aggregates.BankAccountAggregate.BankAccount sender,
                                      Aggregates.BankAccountAggregate.BankAccount recipient,
                                      CashTransaction tx,
                                      decimal amount,
                                      decimal fees)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(recipient);

        EnsureSameCurrencyOrThrow(sender, tx);
        EnsureSameCurrencyOrThrow(recipient, tx);

        EnsureSufficientFundsOrThrow(sender, amount + fees);

        sender.AddAccountTransaction(tx);
        recipient.AddAccountTransaction(tx);

        sender.UpdateBalance(amount + fees, isDeposit: false);
        recipient.UpdateBalance(amount);
    }

    private static void ApplyDeposit(Aggregates.BankAccountAggregate.BankAccount recipient,
                                     CashTransaction tx,
                                     decimal amount)
    {
        ArgumentNullException.ThrowIfNull(recipient);

        EnsureSameCurrencyOrThrow(recipient, tx);

        recipient.AddAccountTransaction(tx);
        recipient.UpdateBalance(amount);
    }

    private static void ApplyWithdrawal(Aggregates.BankAccountAggregate.BankAccount sender,
                                        CashTransaction tx,
                                        decimal amount,
                                        decimal fees)
    {
        ArgumentNullException.ThrowIfNull(sender);

        EnsureSameCurrencyOrThrow(sender, tx);

        EnsureSufficientFundsOrThrow(sender, amount + fees);

        sender.AddAccountTransaction(tx);
        sender.UpdateBalance(amount + fees, isDeposit: false);
    }

    #endregion

    #region Helpers

    private static void EnsureSameCurrencyOrThrow(Aggregates.BankAccountAggregate.BankAccount account, CashTransaction tx)
    {
        if (account.CurrencyId != tx.CurrencyId)
            throw new InvalidOperationException("Transaction currency does not match account currency.");
    }

    private static void EnsureSufficientFundsOrThrow(Aggregates.BankAccountAggregate.BankAccount senderAccount, decimal amountToDeduct)
    {
        var projectedBalance = decimal.Round(senderAccount.Balance - amountToDeduct, 2);
        if (projectedBalance < senderAccount.MinimumAllowedBalance)
            throw new InsufficientFundsException("Insufficient funds to complete the transaction.");
    }

    #endregion
}