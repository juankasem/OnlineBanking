
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Constants;

namespace OnlineBanking.Core.Domain.Services.BankAccount;

public class BankAccountService : IBankAccountService
{
    public bool CreateCashTransaction(Aggregates.BankAccountAggregate.BankAccount senderAccount, 
                                      Aggregates.BankAccountAggregate.BankAccount recipientAccount, 
                                      Guid cashTransactionId, 
                                      decimal amount)
    { 
        var createdTransaction = false;

        if (!Guid.TryParse(cashTransactionId.ToString(), out Guid parsedCashTransactionId) || amount <= 0)
        {
            return createdTransaction;
        }

        //Add transaction to sender's & recipient's accounts
        senderAccount.AddTransaction(AccountTransaction.Create(senderAccount.Id, cashTransactionId));
        recipientAccount.AddTransaction(AccountTransaction.Create(recipientAccount.Id, cashTransactionId));
        
        senderAccount.UpdateBalance(amount, OperationType.Subtract);
        recipientAccount.UpdateBalance(amount, OperationType.Add);

        createdTransaction = true;

        return createdTransaction;
    }

    public bool CreateCustomer(Aggregates.BankAccountAggregate.BankAccount bankAccount, Customer customer)
    {
        bankAccount.AddOwnerToBankAccount(CustomerBankAccount.Create(bankAccount.Id, customer.Id));

        return false;
    }

    public bool CreateFastTransaction(Aggregates.BankAccountAggregate.BankAccount bankAccount, FastTransaction fastTransaction)
    {
        bool createdFastTransaction = false;

        bankAccount.AddFastTransaction(fastTransaction);

        createdFastTransaction = true;

        return createdFastTransaction;
    }
}