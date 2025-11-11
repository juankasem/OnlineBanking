using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineBanking.Infrastructure.Persistence.EntityConfigurations;

public class CashTransactionConfiguration : IEntityTypeConfiguration<CashTransaction>
{
    public void Configure(EntityTypeBuilder<CashTransaction> builder)
    {
        builder.Property(e => e.Id)
         .ValueGeneratedNever();

        builder.Property(ct => ct.Amount)
               .HasPrecision(18, 4);

        builder.Property(ct => ct.Fees)
               .HasPrecision(18, 4);

        builder.Property(ct => ct.SenderAvailableBalance)
        .HasPrecision(18, 4);

        builder.Property(ct => ct.RecipientAvailableBalance)
       .HasPrecision(18, 4);
    }
}
