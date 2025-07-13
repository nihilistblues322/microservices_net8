namespace BuildingBlocks.Messaging.Events;

public record IntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }
    public string EventType { get; init; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().AssemblyQualifiedName ?? GetType().Name;
    }
}