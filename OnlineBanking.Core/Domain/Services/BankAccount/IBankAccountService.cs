
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Core.Domain.Services.BankAccount;

public interface IBankAccountService
{
    bool CreateCashTransaction(Aggregates.BankAccountAggregate.BankAccount senderAccount,
                               Aggregates.BankAccountAggregate.BankAccount recipientAccount,
                               CashTransaction cashTransaction,
                               decimal fees = 0);

    bool CreateFastTransaction(Aggregates.BankAccountAggregate.BankAccount bankAccount, FastTransaction fastTransaction);

    bool DeleteFastTransation(Guid fastTransactionId, Aggregates.BankAccountAggregate.BankAccount bankAccount);

    bool CreateCustomer(Aggregates.BankAccountAggregate.BankAccount bankAccount, Customer customer);
}