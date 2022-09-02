using System;

using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate
{
    public class CustomerBankAccount
    
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        public BankAccountType BankAccountType { get; set; }
    }
}