using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Infrastructure.Persistence.EntityConfigurations
{
    public class AccountTransactionConfiguration : IEntityTypeConfiguration<AccountTransaction>
    {
        public void Configure(EntityTypeBuilder<AccountTransaction> builder)
        {
    //          builder.Property(ac => ac.Account)
    //             .HasForeignKey(ac => ac.AccountId);

    //    builder.Entity<AccountTransaction>()
    //                 .HasOne(ac => ac.Transaction)
    //                 .WithMany(c => c.AccountTransactions)
    //                 .HasForeignKey(c => c.TransactionId);
        }
    }
}