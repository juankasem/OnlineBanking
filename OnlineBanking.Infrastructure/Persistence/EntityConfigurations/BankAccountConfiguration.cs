using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Infrastructure.Persistence.EntityConfigurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.Property(ct => ct.Balance)
           .HasPrecision(18, 4);

        builder.Property(ct => ct.AllowedBalanceToUse)
              .HasPrecision(18, 4);

        builder.Property(ct => ct.MinimumAllowedBalance)
               .HasPrecision(18, 4);

        builder.Property(ct => ct.Debt)
            .HasPrecision(18, 4);


        // Use field access for private collections
        builder.Metadata
            .FindNavigation(nameof(BankAccount.BankAccountOwners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(BankAccount.AccountTransactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(BankAccount.FastTransactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(BankAccount.CreditCards))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(BankAccount.DebitCards))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
