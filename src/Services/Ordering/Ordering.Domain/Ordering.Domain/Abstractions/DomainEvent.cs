namespace Ordering.Domain.Abstractions;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; private set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? GetType().Name;
}