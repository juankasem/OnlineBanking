
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Services.BankAccount;

public class BankAccountService : IBankAccountService
{
    public bool CreateCashTransaction(Aggregates.BankAccountAggregate.BankAccount senderAccount,
                                      Aggregates.BankAccountAggregate.BankAccount recipientAccount,
                                      CashTransaction cashTransaction,
                                      decimal fees = 0)
    {
        var transactionCreated = false;
        var amount = cashTransaction.Amount;

        if (!Guid.TryParse(cashTransaction.Id.ToString(), out _) || amount <= 0)
        {
            return transactionCreated;
        }

        switch (cashTransaction.Type)
        {
            case CashTransactionType.Transfer or CashTransactionType.FAST:

                if (senderAccount is not null && recipientAccount is not null)
                {
                    senderAccount.AddAccountTransaction(cashTransaction);
                    recipientAccount.AddAccountTransaction(cashTransaction);

                    senderAccount.UpdateBalance(amount + fees, OperationType.Subtract);
                    recipientAccount.UpdateBalance(amount, OperationType.Add);
                }
                break;

            case CashTransactionType.Deposit:

                if (recipientAccount is not null)
                {
                    recipientAccount.AddAccountTransaction(cashTransaction);
                    recipientAccount.UpdateBalance(amount, OperationType.Add);
                }
                break;

            case CashTransactionType.Withdrawal:
                if (senderAccount is not null)
                {
                    senderAccount.AddAccountTransaction(cashTransaction);
                    senderAccount.UpdateBalance(amount, OperationType.Subtract);
                }
                break;
            default:
                break;
        }

        transactionCreated = true;

        return transactionCreated;
    }

    public bool CreateCustomer(Aggregates.BankAccountAggregate.BankAccount bankAccount, Customer customer)
    {
        bankAccount.AddOwnerToBankAccount(CustomerBankAccount.Create(bankAccount.Id, customer.Id));

        return false;
    }

    public bool CreateFastTransaction(Aggregates.BankAccountAggregate.BankAccount bankAccount, FastTransaction fastTransaction)
    {
        bool fastTransactionCreated = false;
        if (!Guid.TryParse(fastTransaction.Id.ToString(), out _) || fastTransaction.Amount <= 0)
        {
            return fastTransactionCreated;
        }

        bankAccount.AddFastTransaction(fastTransaction);

        fastTransactionCreated = true;

        return fastTransactionCreated;
    }

    public bool DeleteFastTransation(Guid fastTransactionId, Aggregates.BankAccountAggregate.BankAccount bankAccount)
    {
        bool fastTransactionDeleted = false;
        if (!Guid.TryParse(fastTransactionId.ToString(), out _))
        {
            return fastTransactionDeleted;
        }

        bankAccount.DeleteFastTransaction(fastTransactionId);

        fastTransactionDeleted = true;

        return fastTransactionDeleted;
    }
}