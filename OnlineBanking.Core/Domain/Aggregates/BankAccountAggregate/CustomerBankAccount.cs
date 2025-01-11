using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class CustomerBankAccount
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

    public Guid BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; }

    private CustomerBankAccount(Guid bankAccountId, Guid customerId)
    {
       BankAccountId = bankAccountId;
        CustomerId = customerId;
    }

    public static CustomerBankAccount Create(Guid bankAccountId, Guid customerId) => new(bankAccountId, customerId);
}