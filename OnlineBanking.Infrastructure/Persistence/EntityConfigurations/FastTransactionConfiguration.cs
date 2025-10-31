using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Infrastructure.Persistence.EntityConfigurations;

public class FastTransactionConfiguration : IEntityTypeConfiguration<FastTransaction>
{
    public void Configure(EntityTypeBuilder<FastTransaction> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(ct => ct.Amount)
                    .HasPrecision(18, 4);

        builder.Property(f => f.BankAccountId).IsRequired();

        builder.HasOne(f => f.BankAccount)
               .WithMany( b => b.FastTransactions) 
               .HasForeignKey(f => f.BankAccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
