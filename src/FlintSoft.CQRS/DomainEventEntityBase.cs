using FlintSoft.CQRS.Events;

namespace FlintSoft.CQRS;

public abstract class DomainEventEntityBase
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
