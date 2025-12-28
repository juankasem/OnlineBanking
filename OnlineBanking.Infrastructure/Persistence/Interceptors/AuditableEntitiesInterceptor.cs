using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Infrastructure.Persistence.Interceptors;

public class AuditableEntitiesInterceptor(IAppUserAccessor appUserAccessor) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null)
            return;

        var entries = context.ChangeTracker
        .Entries<IAuditableEntity>()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var currentUser = appUserAccessor.GetDisplayName() ?? "System";
        var currentTime = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser;
                entry.Entity.CreatedOn = currentTime;
            }
            // Always update modified fields for both Added and Modified states
            entry.Entity.LastModifiedBy = currentUser;
            entry.Entity.LastModifiedOn = currentTime;
        }
    }
}

