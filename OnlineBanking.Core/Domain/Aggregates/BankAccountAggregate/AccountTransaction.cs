using System;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class AccountTransaction
{
    public Guid AccountId { get; set; }
    public BankAccount Account { get; set; }

    public Guid TransactionId { get; set; }
    public CashTransaction Transaction { get; set; }
}