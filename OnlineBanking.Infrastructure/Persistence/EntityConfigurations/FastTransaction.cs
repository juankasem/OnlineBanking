using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Infrastructure.Persistence.EntityConfigurations;

public class FastTransactionConfiguration : IEntityTypeConfiguration<FastTransaction>
{
    public void Configure(EntityTypeBuilder<FastTransaction> builder)
    {
        builder.Property(ct => ct.Amount)
                    .HasPrecision(18, 4);
    }
}
