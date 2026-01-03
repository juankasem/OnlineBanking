using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Infrastructure.Services;

namespace OnlineBanking.Infrastructure.Persistence.Interceptors;

public class DispatchDomainEventInterceptor(IMediator mediator, 
             IServiceBusPublisher serviceBusPublisher
    ) 
    : SaveChangesInterceptor
{

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public async Task DispatchDomainEvents(DbContext? dbContext)
    {
        if (dbContext == null)
        {
            return;
        }

        var domainEntities = dbContext.ChangeTracker.Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count != 0)
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEntities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
            await serviceBusPublisher.PublishEventAsync(domainEvent);
        }
    }
}
