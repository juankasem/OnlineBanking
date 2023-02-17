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
            //  builder.Property(ac => ac.Account)
            //         .WithMany(ac => ac.AccountTransactions)
            //     .HasForeignKey(ac => ac.AccountId);

            // builder.Property(ac => ac.Transaction)
            //         .HasOne(ac => ac.Transaction)
            //         .WithMany(c => c.AccountTransactions)
            //         .HasForeignKey(c => c.TransactionId);
        }
    }
}