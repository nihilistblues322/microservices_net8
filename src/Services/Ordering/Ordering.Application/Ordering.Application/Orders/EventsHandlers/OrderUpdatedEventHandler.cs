namespace Ordering.Application.Orders.EventsHandlers;

public class OrderUpdatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    : INotificationHandler<OrderUpdatedEvent>
{
    public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {domainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}