using FlintSoft.CQRS.Events;
using System.Text.Json.Serialization;

namespace FlintSoft.CQRS;

public abstract class DomainEventEntityBase
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [JsonIgnore]
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
