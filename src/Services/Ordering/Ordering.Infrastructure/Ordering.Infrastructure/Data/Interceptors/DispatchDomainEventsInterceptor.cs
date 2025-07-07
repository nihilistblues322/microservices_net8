using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ordering.Infrastructure.Data.Interceptors;

public class DispatchDomainEventsInterceptor(
    IMediator mediator,
    ILogger<DispatchDomainEventsInterceptor> logger)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEventsSync(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void DispatchDomainEventsSync(DbContext? context)
    {
        if (context is null) return;

        var aggregatesWithEvents = context.ChangeTracker
            .Entries<IAggregate>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => new { e.Entity, Events = e.Entity.DomainEvents.ToList() })
            .ToList();

        foreach (var aggregate in aggregatesWithEvents)
        {
            aggregate.Entity.ClearDomainEvents();
        }

        foreach (var domainEvent in aggregatesWithEvents.SelectMany(a => a.Events))
        {
            try
            {
                logger.LogInformation("Publishing domain event (sync): {EventType}", domainEvent.GetType().Name);
                mediator.Publish(domainEvent).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error publishing domain event (sync): {EventType}", domainEvent.GetType().Name);
            }
        }
    }

    private async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) return;

        var aggregatesWithEvents = context.ChangeTracker
            .Entries<IAggregate>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => new { e.Entity, Events = e.Entity.DomainEvents.ToList() })
            .ToList();

        foreach (var aggregate in aggregatesWithEvents)
        {
            aggregate.Entity.ClearDomainEvents();
        }

        foreach (var domainEvent in aggregatesWithEvents.SelectMany(a => a.Events))
        {
            try
            {
                logger.LogInformation("Publishing domain event (async): {EventType}", domainEvent.GetType().Name);
                await mediator.Publish(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error publishing domain event (async): {EventType}", domainEvent.GetType().Name);
            }
        }
    }
}