
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Services.BankAccount;

public interface IBankAccountService
{
    bool CreateCashTransaction(Aggregates.BankAccountAggregate.BankAccount senderAccount, 
                               Aggregates.BankAccountAggregate.BankAccount recipientAccount, 
                               Guid cashTransactionId, 
                               decimal amount,
                               decimal fees,
                               CashTransactionType cashTransactionType);

    bool CreateFastTransaction(Aggregates.BankAccountAggregate.BankAccount bankAccount, FastTransaction fastTransaction);

    bool CreateCustomer(Aggregates.BankAccountAggregate.BankAccount bankAccount, Customer customer);
}