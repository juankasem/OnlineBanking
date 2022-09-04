using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.Infrastructure.Persistence;

public class OnlineBankDbContext : IdentityDbContext<AppUser>
{
    public OnlineBankDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<AccountTransaction> AccountTransactions { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<CashTransaction> CashTransactions { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerBankAccount> CustomerBankAccounts { get; set; }
    public DbSet<DebitCard> DebitCards { get; set; }
    public DbSet<FastTransaction> FastTransactions { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<UtilityPayment> UtilityPayments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineBankDbContext).Assembly);

        modelBuilder.Entity<CustomerBankAccount>()
                    .HasOne(c => c.Customer)
                    .WithMany(c => c.CustomerBankAccounts)
                    .HasForeignKey(c => c.CustomerId);

        modelBuilder.Entity<CustomerBankAccount>()
                    .HasOne(b => b.BankAccount)
                    .WithMany(c => c.BankAccountOwners)
                    .HasForeignKey(c => c.BankAccountId);

        modelBuilder.Entity<AccountTransaction>()
                .HasOne(ac => ac.Account)
                .WithMany(ac => ac.AccountTransactions)
                .HasForeignKey(ac => ac.AccountId);

        modelBuilder.Entity<AccountTransaction>()
                    .HasOne(ac => ac.Transaction)
                    .WithMany(c => c.AccountTransactions)
                    .HasForeignKey(c => c.TransactionId);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseDomainEntity>())
        {
            entry.Entity.LastModifiedOn = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}